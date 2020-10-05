using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Botflox.Bot.Data;
using Botflox.Bot.Services;
using Discord.Commands;
using XivApi;
using XivApi.Character;
using Botflox.Bot.Utils;

namespace Botflox.Bot.Modules
{
    public class CharacterProfileModule : ModuleBase<SocketCommandContext>
    {
        private readonly XivApiClient                     _apiClient;
        private readonly BotfloxDatabase                  _database;
        private readonly FontUtils                        _fontUtils;
        private readonly CharacterProfileGeneratorService _profileGeneratorService;

        public CharacterProfileModule(XivApiClient apiClient, BotfloxDatabase database, FontUtils fontUtils, 
            CharacterProfileGeneratorService profileGeneratorService) {
            _apiClient = apiClient;
            _database = database;
            _fontUtils = fontUtils;
            _profileGeneratorService = profileGeneratorService;
        }

        [Command("profile")]
        public async Task CharacterProfile(ulong lodestoneId) {
            using (Context.Channel.EnterTypingState()) {
                CharacterProfile profile = await _apiClient.CharacterProfileAsync(lodestoneId);
                Image profileImage = await _profileGeneratorService.RenderCharacterProfile(profile, _fontUtils);
                await using MemoryStream memoryStream = new MemoryStream();
                await Task.Run(() => profileImage.Save(memoryStream, ImageFormat.Png));
                memoryStream.Seek(0, SeekOrigin.Begin);
                await Context.Channel.SendFileAsync(memoryStream, $"botfloxProfile.{lodestoneId}.png");
            }
        }
    }
}