using System.Threading.Tasks;
using Botflox.Bot.Data;
using Discord;
using Discord.Commands;

namespace Botflox.Bot.Modules
{
    [RequireOwner(Group = "SettingsMgmt")]
    [RequireUserPermission(GuildPermission.Administrator, Group = "SettingsMgmt")]
    public class GuildSettingsManagementModule : ModuleBase<SocketCommandContext>
    {
        private readonly BotfloxDatabase      _database;

        public GuildSettingsManagementModule(BotfloxDatabase database) {
            _database = database;
        }

        [Command("set-prefix")]
        [RequireContext(ContextType.Guild)]
        public async Task SetPrefixAsync(string prefix) {
            prefix = prefix.TrimStart();
            if (string.IsNullOrWhiteSpace(prefix)) {
                await ReplyAsync("Cannot set empty bot command prefix!");
                return;
            }

            using (Context.Channel.EnterTypingState()) {
                ulong guildId = Context.Guild.Id;
                GuildSettings settings = await _database.GuildsSettings.FindAsync(guildId) ??
                                         new GuildSettings {GuildId = guildId};
                settings.CommandPrefix = prefix;
                _database.Update(settings);
                await _database.SaveChangesAsync();
                await ReplyAsync($"Successfully set botflox command prefix for {Context.Guild.Name} to `{prefix}`");
            }
        }
    }
}