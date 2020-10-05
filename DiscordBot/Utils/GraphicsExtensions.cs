using System.Drawing;

namespace Botflox.Bot.Utils
{
    public static class GraphicsExtensions
    {
        public static void DrawStringCentered(this Graphics graphics, string? str, Font font, Brush brush, float x, float y)
        {
            StringFormat format = StringFormat.GenericDefault;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Far;

            SizeF size = graphics.MeasureString(str, font);
            RectangleF rect = new RectangleF(new PointF(x - size.Width / 2, y - size.Height * 0.75f), size);
            graphics.DrawString(str, font, brush, rect, format);
        }

        public static void DrawString(this Graphics graphics, string? str, Font font, Brush brush, float x, float y)
        {
            StringFormat format = StringFormat.GenericDefault;
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Far;

            SizeF size = graphics.MeasureString(str, font);
            RectangleF rect = new RectangleF(new PointF(x, y - size.Height * 0.75f), size);
            graphics.DrawString(str, font, brush, rect);
        }
    }
}