using System;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using XivApi.Character;
using Botflox.Bot.Utils;

namespace Botflox.Bot.Services
{
    public class CharacterProfileGeneratorService
    {
        private const float JobIconSize    = 49;
        private const float CornerX        = 670;
        private const float CornerY        = 541;
        private const float SpacingX       = 57.1f;
        private const float SpacingY       = 99;
        private const float TextOffsetX    = JobIconSize / 2;
        private const float TextOffsetY    = 72;
        private const float EurekaX        = CornerX + SpacingX * 9;
        private const float EurekaY        = CornerY;
        private const float BozjaX         = CornerX + SpacingX * 10;
        private const float BozjaY         = CornerY;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache       _cache;
        private readonly FontUtils          _fontUtils;

        private readonly Dictionary<int, Tuple<float, float>> _coords = new Dictionary<int, Tuple<float, float>> {
            [33] = Tuple.Create(CornerX, CornerY), // AST
            [24] = Tuple.Create(CornerX + SpacingX, CornerY), // WHM/CNJ
            [28] = Tuple.Create(CornerX + SpacingX * 2, CornerY), // SCH
            [37] = Tuple.Create(CornerX + SpacingX * 4, CornerY), // GNB
            [32] = Tuple.Create(CornerX + SpacingX * 5, CornerY), // DRK
            [21] = Tuple.Create(CornerX + SpacingX * 6, CornerY), // WAR/MRD
            [19] = Tuple.Create(CornerX + SpacingX * 7, CornerY), // PLD/GLA
            [36] = Tuple.Create(CornerX + SpacingX * 11, CornerY), // BLU

            [35] = Tuple.Create(CornerX, CornerY + SpacingY), // RDM
            [25] = Tuple.Create(CornerX + SpacingX, CornerY + SpacingY), // BLM/THM
            [27] = Tuple.Create(CornerX + SpacingX * 2, CornerY + SpacingY), // SMN/ACN
            [23] = Tuple.Create(CornerX + SpacingX * 4, CornerY + SpacingY), // BRD/ARC
            [31] = Tuple.Create(CornerX + SpacingX * 5, CornerY + SpacingY), // MCH
            [38] = Tuple.Create(CornerX + SpacingX * 6, CornerY + SpacingY), // DNC
            [34] = Tuple.Create(CornerX + SpacingX * 8, CornerY + SpacingY), // SAM
            [30] = Tuple.Create(CornerX + SpacingX * 9, CornerY + SpacingY), // NIN/ROG
            [22] = Tuple.Create(CornerX + SpacingX * 10, CornerY + SpacingY), // DRG/LNC
            [20] = Tuple.Create(CornerX + SpacingX * 11, CornerY + SpacingY), // MNK/PGL

            [14] = Tuple.Create(CornerX, CornerY + SpacingY * 2), // ALC
            [15] = Tuple.Create(CornerX + SpacingX, CornerY + SpacingY * 2), // CUL
            [13] = Tuple.Create(CornerX + SpacingX * 2, CornerY + SpacingY * 2), // WVR
            [12] = Tuple.Create(CornerX + SpacingX * 3, CornerY + SpacingY * 2), // LTW
            [8] = Tuple.Create(CornerX + SpacingX * 4, CornerY + SpacingY * 2), // CRP
            [11] = Tuple.Create(CornerX + SpacingX * 5, CornerY + SpacingY * 2), // GSM
            [10] = Tuple.Create(CornerX + SpacingX * 6, CornerY + SpacingY * 2), // ARM
            [9] = Tuple.Create(CornerX + SpacingX * 7, CornerY + SpacingY * 2), // BSM
            [17] = Tuple.Create(CornerX + SpacingX * 9, CornerY + SpacingY * 2), // BTN
            [16] = Tuple.Create(CornerX + SpacingX * 10, CornerY + SpacingY * 2), // MIN
            [18] = Tuple.Create(CornerX + SpacingX * 11, CornerY + SpacingY * 2), // FSH
        };

        private readonly SolidBrush _bWhite  = new SolidBrush(Color.White);
        private readonly SolidBrush _bOrange = new SolidBrush(Color.FromArgb(239, 134, 48));
        private readonly SolidBrush _bGray   = new SolidBrush(Color.LightGray);

        public CharacterProfileGeneratorService(IHttpClientFactory httpClientFactory, 
            IMemoryCache cache, FontUtils fontUtils) 
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _fontUtils = fontUtils;
        }

        private Brush GetLevelColor(bool? isMaxed) {
            return isMaxed == null ? _bGray : (bool) isMaxed ? _bOrange : _bWhite;
        }

        private async Task<Image> GetFreeCompanyCrestImage(HttpClient? client, 
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
                              await _fontUtils.LoadFontResource("Botflox.Bot.Resources.Lato-Bold.ttf");

            Font name = new Font(lato, 48, FontStyle.Bold, GraphicsUnit.Pixel),
                title = new Font(lato, 30, FontStyle.Regular, GraphicsUnit.Pixel),
                medium = new Font(lato, 24, FontStyle.Regular, GraphicsUnit.Pixel),
                small = new Font(lato, 20, FontStyle.Regular, GraphicsUnit.Pixel),
                nameday = new Font(lato, 19, FontStyle.Regular, GraphicsUnit.Pixel),
                number = new Font(lato, 28, FontStyle.Regular, GraphicsUnit.Pixel);

            StringFormat leftAlign = new StringFormat();
            leftAlign.LineAlignment = StringAlignment.Center;
            leftAlign.Alignment = StringAlignment.Near;

            StringFormat centerAlign = new StringFormat();
            centerAlign.LineAlignment = StringAlignment.Center;
            centerAlign.Alignment = StringAlignment.Center;

            HttpClient? client = _httpClientFactory.CreateClient("CharacterProfileGeneration");
            Image portrait, bg = await Task.Run(() => Resources.ProfileBg);
            Image canvas = new Bitmap(1400, 873, PixelFormat.Format32bppArgb);

            // 128x128 here are placeholder values
            Image? crest = null;
            if (profile.FreeCompanyId != null && profile.FreeCompanyCrest != null) {
                string? crestKey = $"crest:{profile.FreeCompanyId}";
                crest = await _cache.GetOrCreateAsync<Image>(crestKey, entry => {
                    entry.SetAbsoluteExpiration(TimeSpan.FromDays(7));
                    return GetFreeCompanyCrestImage(client, profile.FreeCompanyCrest);
                });
            }

            await using (Stream p = await client.GetStreamAsync(profile.Portrait)) {
                portrait = await LoadImageAsync(p);
            }

            using (Graphics graphics = Graphics.FromImage(canvas)) {
                graphics.Clear(Color.White);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                graphics.DrawImage(bg, new Rectangle(640, 0, 762, 873));
                graphics.DrawImage(portrait, new Rectangle(0, 0, 640, 873));

                graphics.DrawString(profile.Name, name, _bWhite, 1008, profile.TitleTop ? 117 : 87, centerAlign);
                graphics.DrawString($"<{profile.Title}>", title, _bWhite, 1008, profile.TitleTop ? 69 : 123, centerAlign);

                graphics.DrawString(profile.GrandCompany?.Name ?? "-", medium, _bWhite, 663, 287, leftAlign);
                graphics.DrawString(profile.GrandCompanyRank ?? "-", small, _bWhite, 718, 317, leftAlign);

                graphics.DrawString($"{profile.Race}, {profile.Tribe}", small, _bWhite, 718, 370, leftAlign);
                graphics.DrawString(profile.GuardianDeity, small, _bWhite, 718, 417, leftAlign);

                graphics.DrawString($"{profile.Server}, {profile.DataCenter}", small, _bWhite, 1071, 371, leftAlign);
                graphics.DrawString(profile.NameDay, nameday, _bWhite, 1071, 417, leftAlign);

                graphics.DrawString(profile.FreeCompanyName ?? "-", medium, _bWhite, 1090, 287, leftAlign);
                graphics.DrawString($"<{profile.FreeCompanyTag}>" ?? "-", small, _bWhite, 1087, 317, leftAlign);

                if (crest != null)
                    graphics.DrawImage(crest, new Rectangle(1020, 270, 64, 64));

                foreach ((int id, (float x, float y)) in _coords) {
                    CharacterClassJobs.ClassJobLevel?
                        level = profile.ClassJobLevels[id];
                    int? jobLevel = level?.Level;
                    bool? jobIsMaxed = level?.IsMaxed;

                    graphics.DrawString(jobLevel?.ToString() ?? "-", number, GetLevelColor(jobIsMaxed), 
                        x + TextOffsetX, y + TextOffsetY, centerAlign);
                    graphics.DrawImage(level?.ClassJob.Icon, new RectangleF(x, y, 49, 49));
                }

                int? eurekaLevel = profile.ContentLevels.ElementalLevel;
                graphics.DrawString(eurekaLevel?.ToString() ?? "-", number,
                    GetLevelColor(profile.ContentLevels.EurekaCapped),
                    EurekaX + TextOffsetX, EurekaY + TextOffsetY, centerAlign);
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