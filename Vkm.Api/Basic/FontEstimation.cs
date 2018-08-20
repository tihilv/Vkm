using System;
using System.Collections.Concurrent;
using System.Drawing;

namespace Vkm.Api.Basic
{
    public static class FontEstimation
    {
        private static readonly ConcurrentDictionary<Tuple<int, int, FontFamily, string>, int> _cache = new ConcurrentDictionary<Tuple<int, int, FontFamily, string>, int>();

        public static int EstimateFontSize(BitmapEx bitmap, FontFamily fontFamily, string sample)
        {
            return _cache.GetOrAdd(new Tuple<int, int, FontFamily, string>(bitmap.Width, bitmap.Height, fontFamily, sample), tuple =>
            {
                using (var graphics = bitmap.CreateGraphics())
                {
                    var height = tuple.Item2;
                    SizeF resultSize;
                    do
                    {
                        using (var font = new Font(tuple.Item3, height, GraphicsUnit.Pixel))
                        {
                            resultSize = graphics.MeasureString(tuple.Item4, font);
                        }

                        height -= 1;
                    } while (resultSize.Height > tuple.Item2 || resultSize.Width > tuple.Item1);

                    return height;
                }
            });
        }
    }
}
