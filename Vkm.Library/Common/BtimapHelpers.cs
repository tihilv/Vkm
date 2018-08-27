using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Layout;

namespace Vkm.Library.Common
{
    static class BitmapHelpers
    {
        public static void ResizeBitmap(BitmapEx source, BitmapEx destination)
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
                    var destRect = new Rectangle(0, 0, destination.Width, destination.Height);
                    graphics.DrawImage(source.GetInternal(), destRect, 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
        }

        public static IEnumerable<LayoutDrawElement> ExtractLayoutDrawElements(BitmapEx source, DeviceSize deviceSize, byte left, byte top, LayoutContext layoutContext)
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

                yield return new LayoutDrawElement(new Location((byte) (left + x), (byte) (top + y)), part);
            }
        }
    }
}
