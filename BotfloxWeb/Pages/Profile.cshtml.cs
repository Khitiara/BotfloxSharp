using System.Threading.Tasks;
using Botflox.Bot.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using XivApi;
using XivApi.Character;

namespace BotfloxWeb.Pages
{
    public class Profile : PageModel
    {
        private readonly XivApiClient _xivApiClient;

        public Profile(XivApiClient xivApiClient) {
            _xivApiClient = xivApiClient;
        }

        public CharacterProfile CharProfile { get; set; } = null!;

        public async Task OnGetAsync(ulong lodestoneId) {
            CharProfile = await _xivApiClient.CharacterProfileAsync(lodestoneId, HttpContext.RequestAborted);
        }
    }
}