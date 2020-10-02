using System;
using System.Threading.Tasks;
using Botflox.Bot.Utils;
using Discord;
using Discord.WebSocket;

namespace Botflox.Bot
{
    public class BotfloxBot
    {
        public DiscordShardedClient Discord { get; }

        private const GuildPermission ExpectedPerms = GuildPermission.ViewChannel | GuildPermission.SendMessages |
                                                      GuildPermission.AttachFiles | GuildPermission.AddReactions;

        public BotfloxBot(DiscordShardedClient discord) {
            Discord = discord;
        }

        public Task<Uri> GetInviteUriAsync() {
            return Discord.GetInviteUriAsync(ExpectedPerms);
        }
    }
}