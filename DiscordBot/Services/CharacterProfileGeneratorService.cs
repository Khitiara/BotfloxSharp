using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using XivApi.Character;
using Botflox.Bot.Utils;

namespace Botflox.Bot.Services
{
    public class CharacterProfileGeneratorService
    {
        private const int CornerX        = 684;
        private const int CornerY        = 658;
        private const int SpacingX       = 48;
        private const int SpacingY       = 83;
        private const int IconOffsetX    = -23;
        private const int IconOffsetY    = -72;
        private const int EurekaX        = CornerX + SpacingX * 12;
        private const int EurekaY        = CornerY;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly FontUtils          _fontUtils;

        private readonly Dictionary<int, Tuple<int, int>> _coords = new Dictionary<int, Tuple<int, int>> {
            [33] = Tuple.Create(CornerX, CornerY),
            [24] = Tuple.Create(CornerX + SpacingX, CornerY),
            [28] = Tuple.Create(CornerX + SpacingX * 2, CornerY),
            [37] = Tuple.Create(CornerX + SpacingX * 4, CornerY),
            [32] = Tuple.Create(CornerX + SpacingX * 5, CornerY),
            [21] = Tuple.Create(CornerX + SpacingX * 6, CornerY),
            [19] = Tuple.Create(CornerX + SpacingX * 7, CornerY),
            [36] = Tuple.Create(CornerX + SpacingX * 11, CornerY),

            [35] = Tuple.Create(CornerX, CornerY + SpacingY),
            [25] = Tuple.Create(CornerX + SpacingX, CornerY + SpacingY),
            [27] = Tuple.Create(CornerX + SpacingX * 2, CornerY + SpacingY),
            [23] = Tuple.Create(CornerX + SpacingX * 4, CornerY + SpacingY),
            [31] = Tuple.Create(CornerX + SpacingX * 5, CornerY + SpacingY),
            [38] = Tuple.Create(CornerX + SpacingX * 6, CornerY + SpacingY),
            [34] = Tuple.Create(CornerX + SpacingX * 8, CornerY + SpacingY),
            [30] = Tuple.Create(CornerX + SpacingX * 9, CornerY + SpacingY),
            [22] = Tuple.Create(CornerX + SpacingX * 10, CornerY + SpacingY),
            [20] = Tuple.Create(CornerX + SpacingX * 11, CornerY + SpacingY),

            [14] = Tuple.Create(CornerX, CornerY + SpacingY * 2),
            [15] = Tuple.Create(CornerX + SpacingX, CornerY + SpacingY * 2),
            [13] = Tuple.Create(CornerX + SpacingX * 2, CornerY + SpacingY * 2),
            [12] = Tuple.Create(CornerX + SpacingX * 3, CornerY + SpacingY * 2),
            [8] = Tuple.Create(CornerX + SpacingX * 4, CornerY + SpacingY * 2),
            [11] = Tuple.Create(CornerX + SpacingX * 5, CornerY + SpacingY * 2),
            [10] = Tuple.Create(CornerX + SpacingX * 6, CornerY + SpacingY * 2),
            [9] = Tuple.Create(CornerX + SpacingX * 7, CornerY + SpacingY * 2),
            [17] = Tuple.Create(CornerX + SpacingX * 9, CornerY + SpacingY * 2),
            [16] = Tuple.Create(CornerX + SpacingX * 10, CornerY + SpacingY * 2),
            [18] = Tuple.Create(CornerX + SpacingX * 11, CornerY + SpacingY * 2),
        };

        private readonly SolidBrush _bWhite  = new SolidBrush(Color.White);
        private readonly SolidBrush _bOrange = new SolidBrush(Color.FromArgb(239, 134, 48));
        private readonly SolidBrush _bGray   = new SolidBrush(Color.LightGray);

        public CharacterProfileGeneratorService(IHttpClientFactory httpClientFactory, FontUtils fontUtils) {
            _httpClientFactory = httpClientFactory;
            _fontUtils = fontUtils;
        }

        private Brush GetLevelColor(bool? isMaxed) {
            return isMaxed == null ? _bGray : (bool) isMaxed ? _bOrange : _bWhite;
        }

        private async ValueTask<Image> GetFreeCompanyCrestImage(HttpClient? client, 
            CharacterProfile.FcCrest? crest) 
        {
            Task<Stream> a = client.GetStreamAsync(crest?.Background);
            Task<Stream> b = client.GetStreamAsync(crest?.Shape);
            Task<Stream> c = client.GetStreamAsync(crest?.Icon);

            Image bgImage, shImage, icImage;

            await using (Stream bgStream = await a)
            await using (Stream shStream = await b)
            await using (Stream icStream = await c)
            {
                bgImage = await LoadImageAsync(bgStream);
                shImage = await LoadImageAsync(shStream);
                icImage = await LoadImageAsync(icStream);
            }

            Bitmap crestImage = new Bitmap(bgImage.Width, bgImage.Height, PixelFormat.Format32bppArgb);
            if (bgImage == null || shImage == null || icImage == null) 
                return crestImage;

            using (Graphics graphics = Graphics.FromImage(crestImage)) {
                graphics.DrawImageUnscaled(bgImage, Point.Empty);
                graphics.DrawImageUnscaled(shImage, Point.Empty);
                graphics.DrawImageUnscaled(icImage, Point.Empty);
            }

            crestImage.MakeTransparent(Color.FromArgb(64, 64, 64));
            return crestImage;
        }

        public async ValueTask<Image> RenderCharacterProfileAsync(CharacterProfile profile) {
            FontFamily lato = await _fontUtils.LoadFontResource("Botflox.Bot.Resources.Lato-Regular.ttf");

            Font big = new Font(lato, 48, FontStyle.Regular, GraphicsUnit.Pixel),
                medium = new Font(lato, 30, FontStyle.Regular, GraphicsUnit.Pixel),
                number = new Font(lato, 28, FontStyle.Regular, GraphicsUnit.Pixel);

            HttpClient? client = _httpClientFactory.CreateClient("CharacterProfileGeneration");
            Image portrait, bg = await Task.Run(() => Resources.ProfileBg);
            Image canvas = new Bitmap(1310, 873, PixelFormat.Format32bppArgb);

            await using (Stream p = await client.GetStreamAsync(profile.Portrait)) {
                portrait = await LoadImageAsync(p);
            }

            using (Graphics graphics = Graphics.FromImage(canvas)) {
                graphics.Clear(Color.White);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                graphics.DrawImage(bg, new Rectangle(640, 0, 670, 873));
                graphics.DrawImage(portrait, new Rectangle(0, 0, 640, 873));

                graphics.DrawStringCentered(profile.Name, big, _bWhite, 974, profile.TitleTop ? 117 : 81);
                graphics.DrawStringCentered($"<{profile.Title}>", medium, _bWhite, 974, profile.TitleTop ? 67 : 124);

                graphics.DrawStringCentered($"{profile.Server} [{profile.DataCenter}]", big, _bWhite, 974, 224);

                graphics.DrawString($"{profile.Race}, {profile.Tribe}", medium, _bWhite, 688, 298);
                graphics.DrawString(profile.GuardianDeity, medium, _bWhite, 688, 361);
                graphics.DrawString(profile.GrandCompanyRank ?? "-", medium, _bWhite, 688, 428);

                string fc = profile.FreeCompanyName != null
                    ? $"{profile.FreeCompanyName} <{profile.FreeCompanyTag}>"
                    : "-";
                graphics.DrawString(fc, medium, _bWhite, 688, 491);

                foreach ((int id, (int x, int y)) in _coords) {
                    CharacterClassJobs.ClassJobLevel?
                        level = profile.ClassJobLevels[id];
                    int? jobLevel = level?.Level;
                    bool? jobIsMaxed = level?.IsMaxed;

                    graphics.DrawStringCentered(jobLevel?.ToString() ?? "-", number, GetLevelColor(jobIsMaxed), x, y);
                    graphics.DrawImage(level?.ClassJob.Icon, new Rectangle(x + IconOffsetX, y + IconOffsetY, 46, 46));
                }

                int? eurekaLevel = profile.ContentLevels.ElementalLevel;
                graphics.DrawStringCentered(eurekaLevel?.ToString() ?? "-", number,
                    GetLevelColor(profile.ContentLevels.EurekaCapped),
                    EurekaX, EurekaY);
            }

            return canvas;
        }

        private static async Task<Image> LoadImageAsync(Stream stream,
            CancellationToken cancellationToken = default) {
            await using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return await Task.Run(() => Image.FromStream(memoryStream), cancellationToken);
        }
    }
}