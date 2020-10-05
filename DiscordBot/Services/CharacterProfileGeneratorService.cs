using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using XivApi.Character;

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

      /*33 to intArrayOf(CORNER_X, CORNER_Y),
      24 to intArrayOf(CORNER_X + SPACING_X, CORNER_Y),
      28 to intArrayOf(CORNER_X + SPACING_X* 2, CORNER_Y),
      37 to intArrayOf(CORNER_X + SPACING_X* 4, CORNER_Y),
      32 to intArrayOf(CORNER_X + SPACING_X* 5, CORNER_Y),
      21 to intArrayOf(CORNER_X + SPACING_X* 6, CORNER_Y),
      19 to intArrayOf(CORNER_X + SPACING_X* 7, CORNER_Y),
      36 to intArrayOf(CORNER_X + SPACING_X* 11, CORNER_Y),

      35 to intArrayOf(CORNER_X, CORNER_Y + SPACING_Y),
      25 to intArrayOf(CORNER_X + SPACING_X, CORNER_Y + SPACING_Y),
      27 to intArrayOf(CORNER_X + SPACING_X* 2, CORNER_Y + SPACING_Y),
      23 to intArrayOf(CORNER_X + SPACING_X* 4, CORNER_Y + SPACING_Y),
      31 to intArrayOf(CORNER_X + SPACING_X* 5, CORNER_Y + SPACING_Y),
      38 to intArrayOf(CORNER_X + SPACING_X* 6, CORNER_Y + SPACING_Y),
      34 to intArrayOf(CORNER_X + SPACING_X* 8, CORNER_Y + SPACING_Y),
      30 to intArrayOf(CORNER_X + SPACING_X* 9, CORNER_Y + SPACING_Y),
      22 to intArrayOf(CORNER_X + SPACING_X* 10, CORNER_Y + SPACING_Y),
      20 to intArrayOf(CORNER_X + SPACING_X* 11, CORNER_Y + SPACING_Y),

      14 to intArrayOf(CORNER_X, CORNER_Y + SPACING_Y* 2),
      15 to intArrayOf(CORNER_X + SPACING_X, CORNER_Y + SPACING_Y* 2),
      13 to intArrayOf(CORNER_X + SPACING_X* 2, CORNER_Y + SPACING_Y* 2),
      12 to intArrayOf(CORNER_X + SPACING_X* 3, CORNER_Y + SPACING_Y* 2),
      8  to intArrayOf(CORNER_X + SPACING_X* 4, CORNER_Y + SPACING_Y* 2),
      11 to intArrayOf(CORNER_X + SPACING_X* 5, CORNER_Y + SPACING_Y* 2),
      10 to intArrayOf(CORNER_X + SPACING_X* 6, CORNER_Y + SPACING_Y* 2),
      9  to intArrayOf(CORNER_X + SPACING_X* 7, CORNER_Y + SPACING_Y* 2),
      17 to intArrayOf(CORNER_X + SPACING_X* 9, CORNER_Y + SPACING_Y* 2),
      16 to intArrayOf(CORNER_X + SPACING_X* 10, CORNER_Y + SPACING_Y* 2),
      18 to intArrayOf(CORNER_X + SPACING_X* 11, CORNER_Y + SPACING_Y* 2)
  )*/

        public CharacterProfileGeneratorService(IHttpClientFactory httpClientFactory) {
            _httpClientFactory = httpClientFactory;
        }

        public async ValueTask<Image> RenderCharacterProfile(CharacterProfile profile) {
            Font nameFont = new Font(FontFamily.GenericSansSerif, 48, FontStyle.Regular, GraphicsUnit.Pixel),
                titleFont = new Font(FontFamily.GenericSansSerif, 30, FontStyle.Regular, GraphicsUnit.Pixel);

            HttpClient? client = _httpClientFactory.CreateClient("CharacterProfileGeneration");
            Image portrait;
            Image bg = await Task.Run(() => Resources.ProfileRight);
            Image canvas = new Bitmap(1310, 873, PixelFormat.Format32bppArgb);

            await using (Stream p = await client.GetStreamAsync(profile.Portrait)) {
                portrait = await LoadImageAsync(p);
            }

            using (Graphics graphics = Graphics.FromImage(canvas)) {
                void DrawCentered(string? str, Font font, Brush brush, float x, float y) {
                    StringFormat format = StringFormat.GenericDefault;
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Far;
                    SizeF size = graphics.MeasureString(str, font);
                    RectangleF rect = new RectangleF(new PointF(x - size.Width / 2, y - size.Height * 0.75f), size);
                    graphics.DrawString(str, font, brush, rect, format);
                }

                void DrawString(string? str, Font font, Brush brush, float x, float y) {
                    StringFormat format = StringFormat.GenericDefault;
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Far;
                    SizeF size = graphics.MeasureString(str, font);
                    RectangleF rect = new RectangleF(new PointF(x, y - size.Height * 0.75f), size);
                    graphics.DrawString(str, font, brush, rect);
                }

                graphics.Clear(Color.White);
                graphics.DrawImage(bg, new Rectangle(640, 0, 670, 873));
                graphics.DrawImage(portrait, new Rectangle(0, 0, 640, 873));

                DrawCentered(profile.Name, nameFont, Brushes.White, 974, profile.TitleTop ? 117 : 81);
                DrawCentered($"<{profile.Title}>", titleFont, Brushes.White, 974, profile.TitleTop ? 67 : 124);

                DrawCentered($"{profile.Server} [{profile.DataCenter}]", nameFont, Brushes.White, 974, 224);
                DrawString($"{profile.Race}, {profile.Tribe}", titleFont, Brushes.White, 688, 327);
                DrawString(profile.GuardianDeity, titleFont, Brushes.White, 688, 390);
                DrawString(profile.GrandCompanyRank ?? "-", titleFont, Brushes.White, 688, 457);
                DrawString(profile.FreeCompanyName ?? "-", titleFont, Brushes.White, 688, 520);

                foreach (var item in coords) {
                    int? jobLevel = profile.ClassJobLevels[item.Key].Value.Level;
                    DrawCentered(jobLevel?.ToString() ?? "-", titleFont, Brushes.White, item.Value.Item1, item.Value.Item2);
                }

                int? eurekaLevel = profile.ContentLevels.ElementalLevel;
                DrawCentered(eurekaLevel?.ToString() ?? "-", titleFont, Brushes.White, EUREKA_X, EUREKA_Y);
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