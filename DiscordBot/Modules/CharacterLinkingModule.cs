using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Botflox.Bot.Data;
using Botflox.Bot.Utils;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using XivApi;
using XivApi.Character;
using XivApi.Character.Raw;

namespace Botflox.Bot.Modules
{
    public class CharacterLinkingModule : ModuleBase<SocketCommandContext>
    {
        private readonly XivApiClient    _apiClient;
        private readonly BotfloxDatabase _database;
        private readonly ReactionAwaiter _reactionAwaiter;

        public CharacterLinkingModule(XivApiClient apiClient, BotfloxDatabase database, DiscordShardedClient client) {
            _apiClient = apiClient;
            _database = database;
            _reactionAwaiter = new ReactionAwaiter(client);
        }

        private async ValueTask<string> GetPrefixAsync(CancellationToken cancellationToken = default) {
            // Are we in a guild or a dm?
            if (!(Context.Channel is SocketTextChannel guildChan)) return "?";
            ulong guildId = guildChan.Guild.Id;
            GuildSettings guildSettings = await _database.GuildsSettings
                .SingleAsync(s => s.GuildId == guildId, cancellationToken);
            return guildSettings.CommandPrefix;
        }

        private async Task WaitForConfirmationAsync(ulong lodestoneId, IUserMessage confirm,
            CharacterProfile profile) {
            bool reacted = false;
            try {
                CancellationTokenSource cts = new CancellationTokenSource(3000);
                await using ReactionAwaiter.Token token = _reactionAwaiter.WaitForReaction(confirm, cts.Token);
                SocketReaction reaction = await token.Task;
                if (reaction.UserId != Context.User.Id) {
                    await ReplyAsync("Only the user executing the command may confirm a character setting");
                    return;
                }

                switch (reaction.Emote.Name) {
                    // Confirmed correct character
                    case "\u2705": {
                        using IDisposable? typingHandle = Context.Channel.EnterTypingState();
                        reacted = true;
                        UserSettings? userSettings;
                        bool stolen = false;
                        if ((userSettings = await _database.UsersSettings.FirstOrDefaultAsync(s =>
                            s.MainCharacter == lodestoneId &&
                            !s.VerifiedCharacter, cts.Token)) != null) {
                            userSettings.VerifiedCharacter = false;
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
                            $"You will now be known as {profile.Name} of {profile.Server}." +
                            "To prevent another user from associating with that character");
                        if (stolen)
                            message.Append(" as you have just done to another");
                        message.Append($", add `botflox:{Context.User.Id}` to your lodestone profile and then run the" +
                                       $" `{prefix}verify` command.");
                        await ReplyAsync(message.ToString());
                        break;
                    }
                    case "\u274E":
                        reacted = true;
                        await ReplyAsync("Try again with a corrected lodestone id");
                        break;
                }
            }
            finally {
                if (!reacted) {
                    await ReplyAsync("Confirmation timed out, try again.");
                }

                await confirm.DeleteAsync();
            }
        }

        [Command("iam")]
        public async Task SetIAmAsync(ulong lodestoneId) {
            IUserMessage confirm;
            CharacterProfile profile;
            using (Context.Channel.EnterTypingState()) {
                profile = await _apiClient.CharacterProfileAsync(lodestoneId);
                if (await _database.UsersSettings.AnyAsync(s => s.MainCharacter == lodestoneId &&
                                                                s.VerifiedCharacter)) {
                    await ReplyAsync($"Character {profile.Name} of {profile.Server} is" +
                                     "already associated with and verified by another Discord user!");
                    return;
                }

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
                if (await _database.UsersSettings.AnyAsync(s => s.MainCharacter == lodestoneId &&
                                                                s.VerifiedCharacter)) {
                    await ReplyAsync($"Character {profile.Name} of {profile.Server} is" +
                                     "already associated with and verified by another Discord user!");
                    return;
                }

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
                                     "necessary, try searching on a specific server with `iams \"First Last\" Cactuar`");
                    return;
                }

                lodestoneId = profile.LodestoneId;
                if (await _database.UsersSettings.AnyAsync(s => s.MainCharacter == lodestoneId &&
                                                                s.VerifiedCharacter)) {
                    await ReplyAsync($"Character {profile.Name} of {profile.Server} is" +
                                     "already associated with and verified by another Discord user!");
                    return;
                }

                confirm = await ReplyAsync($"Found {profile.Name} of {profile.Server}, " +
                                           "react ✅ if so or ❎ if not.");
                await confirm.AddReactionAsync(new Emoji("\u2705"));
                await confirm.AddReactionAsync(new Emoji("\u274E"));
            }

            await WaitForConfirmationAsync(lodestoneId, confirm, profile);
        }
    }
}