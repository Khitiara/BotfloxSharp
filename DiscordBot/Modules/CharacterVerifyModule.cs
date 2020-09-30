using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Botflox.Bot.Data;
using Botflox.Bot.Utils;
using Discord.Commands;
using Discord.WebSocket;
using XivApi;
using XivApi.Character;

namespace Botflox.Bot.Modules
{
    public class CharacterVerifyModule : ModuleBase<SocketCommandContext>
    {
        private readonly XivApiClient    _apiClient;
        private readonly BotfloxDatabase _database;

        public CharacterVerifyModule(XivApiClient apiClient, BotfloxDatabase database) {
            _apiClient = apiClient;
            _database = database;
        }

        private async ValueTask<string> GetPrefixAsync(CancellationToken cancellationToken = default) {
            // Are we in a guild or a dm?
            if (!(Context.Channel is SocketTextChannel guildChan)) return "?";
            ulong guildId = guildChan.Guild.Id;
            GuildSettings guildSettings = await _database.GuildsSettings
                .SingleAsync(s => s.GuildId == guildId, cancellationToken);
            return guildSettings.CommandPrefix;
        }

        [Command("verify-character")]
        public async Task VerifyCharacter() {
            using (Context.Channel.EnterTypingState()) {
                ulong userId = Context.User.Id;
                UserSettings? userSettings = await _database.UsersSettings.FindAsync(userId);
                if (userSettings == null) {
                    string prefix = await GetPrefixAsync();
                    await ReplyAsync($"Your character id has not been set, run `{prefix}iam` to set your " +
                                     "character id first!");
                    return;
                }

                if (userSettings.VerifiedCharacter) {
                    await ReplyAsync("You have already been verified, you dont need to run this!");
                    return;
                }

                CharacterProfile profile = await _apiClient.CharacterProfileAsync(userSettings.MainCharacter);
                if (profile.Bio.Contains($"botflox:{userId}")) {
                    userSettings.VerifiedCharacter = true;
                    _database.Update(userSettings);
                    await _database.SaveChangesAsync();
                    await ReplyAsync($"Successfully verified {Context.User.Mention} as {profile.Name}, you can " +
                                     $"now safely remove `botflox:{userId}` from your lodestone profile.");
                } else {
                    await ReplyAsync($"Could not find a valid verification key in the lodestone profile of " +
                                     $"{profile.Name}, ensure `botflox:{userId}` is present in your lodestone profile " +
                                     "and try again. Note that changes to your lodestone profile may take time to " +
                                     "propagate, but that taking more than a minute or two is rare.");
                }
            }
        }
    }
}