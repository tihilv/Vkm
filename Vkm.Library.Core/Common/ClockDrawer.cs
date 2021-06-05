using System;
using System.Collections.Generic;
using Vkm.Api.Basic;
using Vkm.Api.Common;
using Vkm.Api.Data;
using Vkm.Api.Layout;
using Vkm.Common;

namespace Vkm.Library.Common
{
    public class ClockDrawer
    {
        private byte _first = 99;
        private byte _second = 99;
        private byte _third = 99;

        public IEnumerable<LayoutDrawElement> ProvideDrawElements(byte first, byte second, byte third, GlobalContext globalContext, LayoutContext layoutContext)
        {
            if (_first != first)
            {
                _first = first;
                yield return new LayoutDrawElement(new Location(0, 0), DrawNumber(_first, globalContext, layoutContext));
            }

            if (_second != second)
            {
                _second = second;
                yield return new LayoutDrawElement(new Location(1, 0), DrawNumber(_second, globalContext, layoutContext));
            }

            if (_third != third)
            {
                _third = third;
                yield return new LayoutDrawElement(new Location(2, 0), DrawNumber(_third, globalContext, layoutContext));
            }
        }

        public void Reset()
        {
            _first = 99;
            _second = 99;
            _third = 99;
        }

        private static LazyDictionary<byte, BitmapRepresentation> _cache = new LazyDictionary<Byte, BitmapRepresentation>();

        private BitmapRepresentation DrawNumber(byte number, GlobalContext globalContext, LayoutContext layoutContext)
        {
            var rep = _cache.GetOrAdd(number, b =>
            {
                var bitmap = layoutContext.CreateBitmap();

                var fontFamily = globalContext.Options.Theme.FontFamily;

                var str = b.ToString("00");
                DefaultDrawingAlgs.DrawText(bitmap, fontFamily, str, globalContext.Options.Theme.ForegroundColor);

                var representation = new BitmapRepresentation(bitmap);
                bitmap.Dispose();
                return representation;

            });

            return rep.Clone();
        }
    }
}