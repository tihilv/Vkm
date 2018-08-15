using System.Drawing;
using Vkm.Api.Basic;

namespace Vkm.Api.Layout
{
    public struct LayoutDrawElement
    {
        public readonly Location Location;
        public readonly Bitmap Bitmap;

        public LayoutDrawElement(Location location, Bitmap bitmap)
        {
            Location = location;
            Bitmap = bitmap;
        }
    }
}