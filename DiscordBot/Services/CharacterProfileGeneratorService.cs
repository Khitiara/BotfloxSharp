using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using XivApi.Character;

namespace Botflox.Bot.Services
{
    public class CharacterProfileGeneratorService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CharacterProfileGeneratorService(IHttpClientFactory httpClientFactory) {
            _httpClientFactory = httpClientFactory;
        }

        public async ValueTask<Image> RenderCharacterProfile(CharacterProfile profile) {
            Font nameFont = new Font(FontFamily.GenericSansSerif, 48, FontStyle.Regular, GraphicsUnit.Pixel),
                titleFont = new Font(FontFamily.GenericSansSerif, 30, FontStyle.Regular, GraphicsUnit.Pixel);

            HttpClient? client = _httpClientFactory.CreateClient("CharacterProfileGeneration");
            Image portrait;
            Image rightSide = await Task.Run(() => Resources.ProfileRight);
            Image canvas = new Bitmap(1310, 873, PixelFormat.Format32bppArgb);
            await using (Stream portraitBytes = await client.GetStreamAsync(profile.Portrait)) {
                portrait = await LoadImageAsync(portraitBytes);
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
                graphics.DrawImage(rightSide, new Rectangle(640, 0, 670, 873));
                graphics.DrawImage(portrait, new Rectangle(0, 0, 640, 873));

                DrawCentered(profile.Name, nameFont, Brushes.White, 974, profile.TitleTop ? 117 : 81);
                DrawCentered($"<{profile.Title}>", titleFont, Brushes.White, 974, profile.TitleTop ? 67 : 124);

                DrawCentered($"{profile.Server} [{profile.DataCenter}]", nameFont, Brushes.White, 974, 224);
                DrawString($"{profile.Race}, {profile.Tribe}", titleFont, Brushes.White, 688, 327);
                DrawString(profile.GuardianDeity, titleFont, Brushes.White, 688, 390);
                DrawString(profile.GrandCompanyRank ?? "-", titleFont, Brushes.White, 688, 457);
                DrawString(profile.FreeCompanyName ?? "-", titleFont, Brushes.White, 688, 520);
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