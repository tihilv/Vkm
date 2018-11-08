using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Layout;
using Vkm.Api.Transition;

namespace Vkm.Api.Common
{
    public static class BitmapHelpers
    {
        public static readonly ColorMatrix GrayColorMatrix = new ColorMatrix(
            new[]
            {
                new float[] {.3f, .3f, .3f, 0, 0},
                new float[] {.59f, .59f, .59f, 0, 0},
                new float[] {.11f, .11f, .11f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            });

        public static void ResizeBitmap(BitmapEx source, BitmapEx destination, ColorMatrix colorMatrix = null)
        {
            var destRect = new Rectangle(0, 0, destination.Width, destination.Height);
            ResizeBitmap(source, destination, destRect, colorMatrix);
        }

        public static void ResizeBitmap(BitmapEx source, BitmapEx destination, Rectangle destRectangle, ColorMatrix colorMatrix = null)
        {
            using (var graphics = destination.CreateGraphics())
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);

                    if (colorMatrix != null)
                        wrapMode.SetColorMatrix(colorMatrix);

                    graphics.DrawImage(source.GetInternal(), destRectangle, 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
        }

        public static IEnumerable<LayoutDrawElement> ExtractLayoutDrawElements(BitmapEx source, DeviceSize deviceSize, byte left, byte top, LayoutContext layoutContext, TransitionInfo transitionInfo = default(TransitionInfo))
        {
            var itemWidth = source.Width / deviceSize.Width;
            var itemHeight = source.Height / deviceSize.Height;

            for (byte x = 0; x < deviceSize.Width; x++)
            for (byte y = 0; y < deviceSize.Height; y++)
            {
                var part = layoutContext.CreateBitmap();

                using (Graphics grD = part.CreateGraphics())
                {
                    grD.DrawImage(source.GetInternal(), new Rectangle(0, 0, itemWidth, itemHeight), new Rectangle(x * itemWidth, y * itemHeight, itemWidth, itemHeight), GraphicsUnit.Pixel);
                }

                yield return new LayoutDrawElement(new Location(left + x, top + y), part, transitionInfo);
            }
        }
    }
}
