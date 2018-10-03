using System.Drawing;
using System.Drawing.Imaging;
using Vkm.Api.Basic;
using Vkm.Api.Common;
using Vkm.Api.Options;

namespace Vkm.Api.Device
{
    public static class DeviceHelper
    {
        public static BitmapEx CreateBitmap(this IDevice device, ThemeOptions options)
        {
            var iconSize = device.IconSize;
            var result = new BitmapEx(iconSize.Width, iconSize.Height, PixelFormat.Format24bppRgb);

            if (options.BackgroundBitmapRepresentation == null)
            {
                using (var graphics = result.CreateGraphics())
                using (var brush = new SolidBrush(options.BackgroundColor))
                {
                    graphics.FillRectangle(brush, 0, 0, result.Width, result.Height);
                }
            }
            else
            {
                using (var sourceBitmap = options.BackgroundBitmapRepresentation.CreateBitmap())
                    BitmapHelpers.ResizeBitmap(sourceBitmap, result);
            }

            return result;
        }
    }
}