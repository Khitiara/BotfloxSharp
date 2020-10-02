using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Botflox.Bot.Utils
{
    public static class DiscordExtensions
    {
        public static Task LogDiscord(this ILogger logger, LogMessage message) {
            static LogLevel LevelConvert(LogSeverity severity) =>
                severity switch {
                    LogSeverity.Critical => LogLevel.Critical,
                    LogSeverity.Error => LogLevel.Error,
                    LogSeverity.Warning => LogLevel.Warning,
                    LogSeverity.Info => LogLevel.Information,
                    LogSeverity.Verbose => LogLevel.Trace,
                    LogSeverity.Debug => LogLevel.Debug,
                    _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
                };

            string msg = string.IsNullOrEmpty(message.Source)
                ? message.Message
                : $"{message.Source.PadRight(11)} {message.Message}";
            return Task.Run(() => logger.Log(LevelConvert(message.Severity), message.Exception, "{timestamp} {msg}",
                DateTime.Now,
                msg));
        }

        public static async Task<Uri> GetInviteUriAsync(this BaseSocketClient client, GuildPermission permissions = 0) {
            RestApplication app = await client.GetApplicationInfoAsync();
            StringBuilder builder = new StringBuilder("https://discord.com/oauth2/authorize?scope=bot&client_id=");
            builder.Append(app.Id);
            if (permissions != 0) {
                builder.Append("&permissions=").Append((ulong) permissions);
            }

            return new Uri(builder.ToString());
        }
    }
}