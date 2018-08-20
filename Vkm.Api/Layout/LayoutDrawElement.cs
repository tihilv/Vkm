using System.Drawing;
using Vkm.Api.Basic;

namespace Vkm.Api.Layout
{
    public struct LayoutDrawElement
    {
        public readonly Location Location;
        public readonly BitmapRepresentation BitmapRepresentation;

        public LayoutDrawElement(Location location, BitmapEx bitmap)
        {
            Location = location;
            BitmapRepresentation = new BitmapRepresentation(bitmap);
            bitmap.Dispose();
        }

        public LayoutDrawElement(Location location, BitmapRepresentation bitmapRepresentation)
        {
            Location = location;
            BitmapRepresentation = bitmapRepresentation;
        }
    }
}