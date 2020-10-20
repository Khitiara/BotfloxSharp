using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Botflox.Bot.Modules
{
    public class LinksModule : ModuleBase<SocketCommandContext>
    {
        private readonly BotfloxBot _bot;

        public LinksModule(BotfloxBot bot) {
            _bot = bot;
        }

        [Command("botflox")]
        public async Task BotfloxWebsiteAsync() {
            await ReplyAsync("More info about Botflox can be found at https://botflox.catboi.world");
        }

        [Command("invite")]
        public async Task BotfloxInviteAsync() {
            Uri invite = await _bot.GetInviteUriAsync();
            await ReplyAsync(invite.ToString());
        }
    }
}