using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Botflox.Bot.Data;
using Botflox.Bot.Utils;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XivApi;
using XivApi.Character;
using Microsoft.EntityFrameworkCore;

namespace Botflox.Bot.Modules
{
    public class CharacterLinkingModule : ModuleBase<SocketCommandContext>
    {
        private readonly XivApiClient    _apiClient;
        private readonly BotfloxDatabase _database;
        private readonly ReactionAwaiter _reactionAwaiter;

        // ReSharper disable once SuggestBaseTypeForParameter
        public CharacterLinkingModule(XivApiClient apiClient, BotfloxDatabase database, DiscordShardedClient client) {
            _apiClient = apiClient;
            _database = database;
            _reactionAwaiter = new ReactionAwaiter(client);
        }

        private async ValueTask<string> GetPrefixAsync(CancellationToken cancellationToken = default) {
            // Are we in a guild or a dm?
            if (!(Context.Channel is SocketTextChannel guildChan)) return "?";
            ulong guildId = guildChan.Guild.Id;
            GuildSettings guildSettings = await ((IQueryable<GuildSettings>) _database.GuildsSettings)
                .SingleAsync(s => s.GuildId == guildId, cancellationToken);
            return guildSettings.CommandPrefix;
        }

        private async Task WaitForConfirmationAsync(ulong lodestoneId, IUserMessage confirm,
            CharacterProfile profile) {
            bool reacted = false;
            try {
                CancellationTokenSource cts = new CancellationTokenSource(10000);
                using ReactionAwaiter.Token token = _reactionAwaiter.WaitForReaction(confirm,
                    out ChannelReader<SocketReaction> reactionChannel, cts.Token);
                await foreach (SocketReaction reaction in reactionChannel.ReadAllAsync(cts.Token)) {
                    if (reaction == null) continue;

                    if (reaction.UserId != Context.User.Id) {
                        await ReplyAsync("Only the user executing the command may confirm a character setting, " +
                                         MentionUtils.MentionUser(reaction.UserId));
                        return;
                    }

                    switch (reaction.Emote.Name) {
                        // Confirmed correct character
                        case "\u2705": {
                            using IDisposable? typingHandle = Context.Channel.EnterTypingState();
                            reacted = true;
                            UserSettings? userSettings;
                            bool stolen = false;
                            if ((userSettings = await ((IQueryable<UserSettings>) _database.UsersSettings)
                                .FirstOrDefaultAsync(s => s.MainCharacter == lodestoneId &&
                                                          !s.VerifiedCharacter, cts.Token)) != null) {
                                _database.Remove(userSettings);
                                stolen = true;
                            }

                            userSettings = await _database.UsersSettings.FindAsync(Context.User.Id);
                            if (userSettings == null) {
                                userSettings = new UserSettings {
                                    UserId = Context.User.Id
                                };
                                await _database.UsersSettings.AddAsync(userSettings, cts.Token);
                            }

                            userSettings.MainCharacter = lodestoneId;
                            userSettings.VerifiedCharacter = false;
                            await _database.SaveChangesAsync(cts.Token);

                            string prefix = await GetPrefixAsync(cts.Token);
                            StringBuilder message = new StringBuilder(
                                $"You will now be known as {profile.Name} of {profile.Server}. " +
                                "To prevent another user from associating with that character,");

                            if (stolen)
                                message.Append(" as you have just done to another,");

                            message.Append(
                                $" add `botflox:{Context.User.Id}` to your Lodestone profile and then run the" +
                                $" `{prefix}verify-character` command.");

                            await confirm.DeleteAsync();
                            await ReplyAsync(message.ToString());
                            return;
                        }
                        case "\u274E":
                            reacted = true;
                            await confirm.DeleteAsync();
                            await ReplyAsync("Try again with a corrected Lodestone ID.");
                            return;
                    }
                }
            }
            finally {
                _reactionAwaiter.Dispose();

                if (!reacted) {
                    await confirm.DeleteAsync();
                    await ReplyAsync("Confirmation timed out, please try again.");
                }
            }
        }

        private async Task<bool> CheckConflict(ulong lodestoneId, CharacterProfile profile) {
            UserSettings? settings = await ((IQueryable<UserSettings>) _database.UsersSettings).FirstOrDefaultAsync(s =>
                s.MainCharacter == lodestoneId);
            if (settings == null) return false;
            if (settings.UserId == Context.User.Id) {
                if (settings.VerifiedCharacter) {
                    await ReplyAsync($"You are already associated with {profile.Name} of {profile.Server}");
                    return true;
                }

                string prefix = await GetPrefixAsync();
                await ReplyAsync($"You are already pending verification as {profile.Name} of " +
                                 $"{profile.Server}, run `{prefix}verify-character` to start the " +
                                 $"verification process.");
                return true;
            }

            if (!settings.VerifiedCharacter) return false;
            await ReplyAsync($"Character {profile.Name} of {profile.Server} is" +
                             "already associated with and verified by another Discord user!");
            return true;
        }

        [Command("iam")]
        public async Task SetIAmAsync(ulong lodestoneId) {
            IUserMessage confirm;
            CharacterProfile profile;
            using (Context.Channel.EnterTypingState()) {
                profile = await _apiClient.CharacterProfileAsync(lodestoneId);
                if (await CheckConflict(lodestoneId, profile)) return;

                confirm = await ReplyAsync($"Found {profile.Name} of {profile.Server}, " +
                                           "react ✅ if so or ❎ if not.");
                await confirm.AddReactionAsync(new Emoji("\u2705"));
                await confirm.AddReactionAsync(new Emoji("\u274E"));
            }

            await WaitForConfirmationAsync(lodestoneId, confirm, profile);
        }

        [Command("iam")]
        public async Task SetIAmAsync([Remainder] string name) {
            IUserMessage confirm;
            CharacterProfile profile;
            ulong lodestoneId;
            using (Context.Channel.EnterTypingState()) {
                try {
                    profile = await _apiClient.FindSingleCharacterAsync(name);
                }
                catch (InvalidOperationException) {
                    string prefix = await GetPrefixAsync();
                    await ReplyAsync("Search terms returned either multiple or no characters, try again. If " +
                                     "necessary, try searching on a specific server with " +
                                     $"`{prefix}iams \"First Last\" Server`");
                    return;
                }

                lodestoneId = profile.LodestoneId;
                if (await CheckConflict(lodestoneId, profile)) return;

                confirm = await ReplyAsync($"Found {profile.Name} of {profile.Server}, " +
                                           "react ✅ if so or ❎ if not.");
                await confirm.AddReactionAsync(new Emoji("\u2705"));
                await confirm.AddReactionAsync(new Emoji("\u274E"));
            }

            await WaitForConfirmationAsync(lodestoneId, confirm, profile);
        }

        [Command("iams")]
        public async Task SetIAmAsync(string name, string server) {
            IUserMessage confirm;
            CharacterProfile profile;
            ulong lodestoneId;
            using (Context.Channel.EnterTypingState()) {
                try {
                    profile = await _apiClient.FindSingleCharacterAsync(name, server);
                }
                catch (InvalidOperationException) {
                    await ReplyAsync("Search terms returned either multiple or no characters, try again. If " +
                                     "necessary, try searching on a specific server with `iams \"First Last\" Server`");
                    return;
                }

                lodestoneId = profile.LodestoneId;
                if (await CheckConflict(lodestoneId, profile)) return;

                confirm = await ReplyAsync($"Found {profile.Name} of {profile.Server}, " +
                                           "react ✅ if so or ❎ if not.");
                await confirm.AddReactionAsync(new Emoji("\u2705"));
                await confirm.AddReactionAsync(new Emoji("\u274E"));
            }

            await WaitForConfirmationAsync(lodestoneId, confirm, profile);
        }
    }
}