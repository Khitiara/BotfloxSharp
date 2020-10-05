using System.Threading.Tasks;
using Botflox.Bot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BotfloxWeb.Pages
{
    public class Invite : PageModel
    {
        private readonly BotfloxBot _botflox;

        public Invite(BotfloxBot botflox) {
            _botflox = botflox;
        }

        public async Task<IActionResult> OnGetAsync() {
            return Redirect((await _botflox.GetInviteUriAsync()).ToString());
        }
    }
}