using Vkm.Api.Basic;
using Vkm.Api.VisualEffect;

namespace Vkm.Core.VisualEffect
{
    internal class VisualEffectInfo
    {
        private readonly Location _location;
        private IVisualTransition _transition;

        private BitmapRepresentation _first;
        private BitmapRepresentation _last;

        public Location Location => _location;
        public IVisualTransition Transition => _transition;

        public VisualEffectInfo(Location location, IVisualTransition transition, BitmapRepresentation first, BitmapRepresentation last, int steps)
        {
            _location = location;
            _transition = transition;
            _first = first;
            _last = last;

            transition.Init(first, last, steps);
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndNull(ref _first);
            DisposeHelper.DisposeAndNull(ref _last);

            DisposeHelper.DisposeAndNull(ref _transition);
        }

        public void ReplaceLastBitmap(BitmapRepresentation lastBitmapRepresentation)
        {
            _transition.ReplaceLastBitmap(lastBitmapRepresentation);
            DisposeHelper.DisposeAndNull(ref _last);
            _last = lastBitmapRepresentation;
        }
    }
}