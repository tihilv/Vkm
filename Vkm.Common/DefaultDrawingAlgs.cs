using System;
using System.Drawing;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Options;

namespace Vkm.Common
{
    public static class DefaultDrawingAlgs
    {
        public static void DrawIconAndText(BitmapEx bitmap, FontFamily iconFontFamily, string iconSymbol, FontFamily textFontFamily, string text, string textExample, Color color)
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

        public static void DrawCaptionedIcon(BitmapEx bitmap, FontFamily iconFontFamily, string iconSymbol, FontFamily textFontFamily, string text, string textExample, Color color)
        {
            DrawElements(bitmap, new TextDrawElement(iconFontFamily, iconSymbol, color, 70, StringAlignment.Center), new TextDrawElement(textFontFamily, text, color, 30, StringAlignment.Center));
        }

        internal static void DrawElements(BitmapEx bitmap, params TextDrawElement[] elements)
        {
            using (var graphics = bitmap.CreateGraphics())
            {
                int yPosition = 0;
                for (var index = 0; index < elements.Length; index++)
                {
                    TextDrawElement element = elements[index];

                    var maxHeight = bitmap.Height * element.Percentage / 100;
                    var textHeight = (element.Text != null)?FontEstimation.EstimateFontSize(bitmap, element.FontFamily, element.Text, alterHeight:maxHeight):0;

                    using (var brush = new SolidBrush(element.Color))
                    using (Font textFont = (textHeight > 0)?new Font(element.FontFamily, textHeight, GraphicsUnit.Pixel): null)
                    {
                        var size = graphics.MeasureString(element.Text, textFont);

                        float x = 0;
                        if (element.Alignment == StringAlignment.Center)
                            x = (bitmap.Width - size.Width) / 2;
                        else if (element.Alignment == StringAlignment.Far)
                            x = (bitmap.Width - size.Width);

                        graphics.DrawString(element.Text, textFont, brush, x, yPosition + (maxHeight - size.Height) / 2);
                    }

                    yPosition += maxHeight;
                }
            }
        }

        public static void DrawTexts(BitmapEx bitmap, FontFamily fontFamily, string l1, string l2, string textExample, Color color)
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

        public static void DrawText(BitmapEx bitmap, FontFamily fontFamily, string text, Color color)
        {            
            var height = FontEstimation.EstimateFontSize(bitmap, fontFamily, text);

            using (var graphics = bitmap.CreateGraphics())
            using (var whiteBrush = new SolidBrush(color))
            using (var font = new Font(fontFamily, height, GraphicsUnit.Pixel))
            {
                var size = graphics.MeasureString(text, font);
                if (size.Width / size.Height > 5)
                {
                    var splittedText = SplitText(text);
                    if (splittedText != text)
                    {
                        DrawText(bitmap, fontFamily, splittedText, color);
                        return;
                    }
                }

                graphics.DrawString(text, font, whiteBrush, (bitmap.Width - size.Width) / 2, (bitmap.Height - size.Height) / 2);
            }
        }

        public static void DrawPlot(BitmapEx bitmap, Color color, int[] values, int fromY, int toY, int startIndex = 0, double? minValue = null, double? maxValue = null)
        {
            DrawPlot(bitmap, color, values.Select(v=>(double)v).ToArray(), fromY, toY, startIndex, minValue, maxValue);
        }
        
        public static void DrawPlot(BitmapEx bitmap, Color color, double[] values, int fromY, int toY, int startIndex = 0, double? minValue = null, double? maxValue = null)
        {
            var min = double.MaxValue;
            var max = double.MinValue;

            if (minValue == null || maxValue == null)
            {
                for (int i = startIndex; i < values.Length; i++)
                {
                    if (min > values[i])
                        min = values[i];
            
                    if (max < values[i])
                        max = values[i];
                }
            }
            
            min = minValue ?? min;
            max = maxValue ?? max;

            var coef = (max != min) ? ((toY - fromY) / ((max - min))) : 0;

            var midY = (toY + fromY) / 2;
            var midValue = (max + min) / 2.0;
            
            Point[] points = new Point[values.Length - startIndex];
            for (int i = startIndex; i < values.Length; i++)
            {
                points[i-startIndex] = new Point(i-startIndex, (int)(midY - (values[i]-midValue)*coef));
            }

            using (var graphics = bitmap.CreateGraphics())
            using (var pen = new Pen(color))
            {
                graphics.DrawCurve(pen, points);
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
                text = text.Substring(0, pos) + "\n" + text.Substring(pos + 1);
            }

            return text;
        }

        public static void SelectElement(BitmapEx bitmap, ThemeOptions themeOptions)
        {
            using (var graphics = bitmap.CreateGraphics())
            using (var pen = new Pen(themeOptions.ForegroundColor, 3))
            {
                graphics.DrawRectangle(pen, 0, 0, bitmap.Width, bitmap.Height);
            }

        }
    }
}
