using System;
using System.Threading.Tasks;
using Botflox.Bot;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BotfloxWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly BotfloxBot          _botflox;

        public IndexModel(ILogger<IndexModel> logger, BotfloxBot botflox) {
            _logger = logger;
            _botflox = botflox;
        }

        public Uri InviteUri { get; set; }

        public async Task OnGetAsync() {
            InviteUri = await _botflox.GetInviteUriAsync();
        }
    }
}