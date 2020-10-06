using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Botflox.Bot.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using XivApi;
using XivApi.Character;

namespace BotfloxWeb.Pages
{
    public class Profile : PageModel
    {
        private readonly XivApiClient                     _xivApiClient;
        private readonly CharacterProfileGeneratorService _profileGenerator;

        public Profile(XivApiClient xivApiClient, CharacterProfileGeneratorService profileGenerator) {
            _xivApiClient = xivApiClient;
            _profileGenerator = profileGenerator;
        }

        public CharacterProfile CharProfile { get; set; } = null!;
        public string ProfileImageB64 { get; set; } = null!;

        public async Task OnGetAsync(ulong lodestoneId) {
            CharProfile = await _xivApiClient.CharacterProfileAsync(lodestoneId, HttpContext.RequestAborted);
            Image image = await _profileGenerator.RenderCharacterProfileAsync(CharProfile);
            await using MemoryStream memoryStream = new MemoryStream();
            await Task.Run(() => image.Save(memoryStream, ImageFormat.Png));
            ProfileImageB64 = Convert.ToBase64String(memoryStream.GetBuffer());
        }
    }
}