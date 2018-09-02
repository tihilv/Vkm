using System;
using System.Drawing;
using Vkm.Api.Basic;

namespace Vkm.Library.Common
{
    static class DefaultDrawingAlgs
    {
        internal static void DrawIconAndText(BitmapEx bitmap, FontFamily iconFontFamily, string iconSymbol, FontFamily textFontFamily, string text, string textExample, Color color)
        {
            var textHeight = (text != null)?FontEstimation.EstimateFontSize(bitmap, textFontFamily, textExample):0;
            var imageHeight = (int) (bitmap.Height * 0.9 - textHeight);

            using (var graphics = bitmap.CreateGraphics())
            using (var whiteBrush = new SolidBrush(color))
            using (Font textFont = (textHeight > 0)?new Font(textFontFamily, textHeight, GraphicsUnit.Pixel): null)
            using (Font imageFont = new Font(iconFontFamily, imageHeight, GraphicsUnit.Pixel))
            {

                var size = graphics.MeasureString(text, textFont);
                graphics.DrawString(text, textFont, whiteBrush, bitmap.Width - size.Width, bitmap.Height - size.Height);

                graphics.DrawString(iconSymbol, imageFont, whiteBrush, 0, 0);
            }
        }

        internal static void DrawTexts(BitmapEx bitmap, FontFamily fontFamily, string l1, string l2, string textExample, Color color)
        {
            var textHeight = FontEstimation.EstimateFontSize(bitmap, fontFamily, textExample);

            using (var graphics = bitmap.CreateGraphics())
            using (var whiteBrush = new SolidBrush(color))
            using (var textFont = new Font(fontFamily, textHeight, GraphicsUnit.Pixel))
            {
                var size = graphics.MeasureString(l1, textFont);
                graphics.DrawString(l1, textFont, whiteBrush, bitmap.Width - size.Width, 0);

                size = graphics.MeasureString(l2, textFont);
                graphics.DrawString(l2, textFont, whiteBrush, bitmap.Width - size.Width, bitmap.Height / 2);
            }
        }

        internal static void DrawText(BitmapEx bitmap, FontFamily fontFamily, string text, Color color)
        {            
            var height = FontEstimation.EstimateFontSize(bitmap, fontFamily, text);

            using (var graphics = bitmap.CreateGraphics())
            using (var whiteBrush = new SolidBrush(color))
            using (var font = new Font(fontFamily, height, GraphicsUnit.Pixel))
            {
                var size = graphics.MeasureString(text, font);
                if (size.Width / size.Height > 10)
                    DrawText(bitmap, fontFamily, SplitText(text), color);
                else
                    graphics.DrawString(text, font, whiteBrush, (bitmap.Width - size.Width) / 2, (bitmap.Height - size.Height) / 2);
            }

        }

        private static string SplitText(string text)
        {
            int pos = 0;
            for (int i = 0; i < Math.Min(text.Length, text.Length-pos); i++)
            {
                if (text[i] == ' ')
                    pos = i;
            }

            if (pos > 0)
            {
                text = text.Substring(0, pos - 1) + "\n" + text.Substring(pos + 1);
            }

            return text;
        }
    }
}
