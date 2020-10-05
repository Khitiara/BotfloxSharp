using Botflox.Bot.Data;

namespace BotfloxWeb.Models
{
    public class GuildSettingsDetailModel
    {
        public GuildSettingsDetailModel(GuildSettings settings, string guildName) {
            Settings = settings;
            GuildName = guildName;
        }
        public GuildSettings Settings { get; set; }
        public string GuildName { get; }
    }
}