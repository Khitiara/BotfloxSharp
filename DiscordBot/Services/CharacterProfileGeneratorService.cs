﻿using System;
using System.Drawing;
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
        internal const int CORNER_X = 684;
        internal const int CORNER_Y = 658;
        internal const int SPACING_X = 48;
        internal const int SPACING_Y = 83;
        internal const int EUREKA_X = CORNER_X + SPACING_X * 12;
        internal const int EUREKA_Y = CORNER_Y;
        internal const int EUREKA_MAXLVL = 60;

        private readonly IHttpClientFactory _httpClientFactory;

        internal Dictionary<int, Tuple<int, int>> coords = new Dictionary<int, Tuple<int, int>>() 
        {
            [33] = new (CORNER_X,                  CORNER_Y),
            [24] = new (CORNER_X + SPACING_X,      CORNER_Y),
            [28] = new (CORNER_X + SPACING_X * 2,  CORNER_Y),
            [37] = new (CORNER_X + SPACING_X * 4,  CORNER_Y),
            [32] = new (CORNER_X + SPACING_X * 5,  CORNER_Y),
            [21] = new (CORNER_X + SPACING_X * 6,  CORNER_Y),
            [19] = new (CORNER_X + SPACING_X * 7,  CORNER_Y),
            [36] = new (CORNER_X + SPACING_X * 11, CORNER_Y),

            [35] = new (CORNER_X,                  CORNER_Y + SPACING_Y),
            [25] = new (CORNER_X + SPACING_X,      CORNER_Y + SPACING_Y),
            [27] = new (CORNER_X + SPACING_X * 2,  CORNER_Y + SPACING_Y),
            [23] = new (CORNER_X + SPACING_X * 4,  CORNER_Y + SPACING_Y),
            [31] = new (CORNER_X + SPACING_X * 5,  CORNER_Y + SPACING_Y),
            [38] = new (CORNER_X + SPACING_X * 6,  CORNER_Y + SPACING_Y),
            [34] = new (CORNER_X + SPACING_X * 8,  CORNER_Y + SPACING_Y),
            [30] = new (CORNER_X + SPACING_X * 9,  CORNER_Y + SPACING_Y),
            [22] = new (CORNER_X + SPACING_X * 10, CORNER_Y + SPACING_Y),
            [20] = new (CORNER_X + SPACING_X * 11, CORNER_Y + SPACING_Y),

            [14] = new (CORNER_X,                  CORNER_Y + SPACING_Y * 2),
            [15] = new (CORNER_X + SPACING_X,      CORNER_Y + SPACING_Y * 2),
            [13] = new (CORNER_X + SPACING_X * 2,  CORNER_Y + SPACING_Y * 2),
            [12] = new (CORNER_X + SPACING_X * 3,  CORNER_Y + SPACING_Y * 2),
            [ 8] = new (CORNER_X + SPACING_X * 4,  CORNER_Y + SPACING_Y * 2),
            [11] = new (CORNER_X + SPACING_X * 5,  CORNER_Y + SPACING_Y * 2),
            [10] = new (CORNER_X + SPACING_X * 6,  CORNER_Y + SPACING_Y * 2),
            [ 9] = new (CORNER_X + SPACING_X * 7,  CORNER_Y + SPACING_Y * 2),
            [17] = new (CORNER_X + SPACING_X * 9,  CORNER_Y + SPACING_Y * 2),
            [16] = new (CORNER_X + SPACING_X * 10, CORNER_Y + SPACING_Y * 2),
            [18] = new (CORNER_X + SPACING_X * 11, CORNER_Y + SPACING_Y * 2),
        };

        internal SolidBrush bWhite = new SolidBrush(Color.White);
        internal SolidBrush bOrange = new SolidBrush(Color.FromArgb(239, 134, 48));
        internal SolidBrush bGray = new SolidBrush(Color.LightGray);

        public CharacterProfileGeneratorService(IHttpClientFactory httpClientFactory) {
            _httpClientFactory = httpClientFactory;
        }

        private Brush GetLevelColor(bool? isMaxed) {
            return (isMaxed == null) ? bGray : ((bool) isMaxed ? bOrange : bWhite);
        }

        public async ValueTask<Image> RenderCharacterProfile(CharacterProfile profile, FontUtils fontUtils) {
            FontFamily lato = await fontUtils.LoadFontResource("Botflox.Bot.Resources.Lato-Regular.ttf");

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
                graphics.DrawImage(bg, new Rectangle(640, 0, 670, 873));
                graphics.DrawImage(portrait, new Rectangle(0, 0, 640, 873));

                graphics.DrawStringCentered(profile.Name, big, bWhite, 974, profile.TitleTop ? 117 : 81);
                graphics.DrawStringCentered($"<{profile.Title}>", medium, bWhite, 974, profile.TitleTop ? 67 : 124);

                graphics.DrawStringCentered($"{profile.Server} [{profile.DataCenter}]", big, bWhite, 974, 224);

                graphics.DrawString($"{profile.Race}, {profile.Tribe}", medium, bWhite, 688, 298);
                graphics.DrawString(profile.GuardianDeity, medium, bWhite, 688, 361);
                graphics.DrawString(profile.GrandCompanyRank ?? "-", medium, bWhite, 688, 428);

                string fc = (profile.FreeCompanyName != null) ? $"{profile.FreeCompanyName} <{profile.FreeCompanyTag}>" : "-";
                graphics.DrawString(fc, medium, bWhite, 688, 491);

                foreach (var item in coords) {
                    int?  jobLevel   = profile.ClassJobLevels[item.Key].Value.Level;
                    bool? jobIsMaxed = profile.ClassJobLevels[item.Key].Value.IsMaxed;

                    graphics.DrawStringCentered(jobLevel?.ToString() ?? "-", number, GetLevelColor(jobIsMaxed), item.Value.Item1, item.Value.Item2);
                }

                int? eurekaLevel = profile.ContentLevels.ElementalLevel;
                bool? eurekaIsMaxed = (eurekaLevel == null) ? null : (eurekaLevel == EUREKA_MAXLVL);
                graphics.DrawStringCentered(eurekaLevel?.ToString() ?? "-", number, GetLevelColor(eurekaIsMaxed), EUREKA_X, EUREKA_Y);
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