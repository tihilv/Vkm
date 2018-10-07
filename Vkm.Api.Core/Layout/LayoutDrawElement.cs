using Vkm.Api.Basic;
using Vkm.Api.Transition;

namespace Vkm.Api.Layout
{
    public struct LayoutDrawElement
    {
        public readonly Location Location;
        public readonly BitmapRepresentation BitmapRepresentation;
        public readonly TransitionInfo TransitionInfo;

        public LayoutDrawElement(Location location, BitmapEx bitmap, TransitionInfo transitionInfo = default(TransitionInfo))
        {
            Location = location;
            TransitionInfo = transitionInfo;
            BitmapRepresentation = new BitmapRepresentation(bitmap);
            bitmap.Dispose();
        }

        public LayoutDrawElement(Location location, BitmapRepresentation bitmapRepresentation, TransitionInfo transitionInfo = default(TransitionInfo))
        {
            Location = location;
            BitmapRepresentation = bitmapRepresentation;
            TransitionInfo = transitionInfo;
        }
    }
}