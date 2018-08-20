using Vkm.Api.Basic;
using Vkm.Api.VisualEffect;

namespace Vkm.Core.VisualEffect
{
    internal class VisualEffectInfo
    {
        private readonly Location _location;
        private readonly IVisualTransition _transition;

        private readonly BitmapRepresentation _first;
        private readonly BitmapRepresentation _last;

        public Location Location => _location;
        public IVisualTransition Transition => _transition;

        public VisualEffectInfo(Location location, IVisualTransition transition, BitmapRepresentation first, BitmapRepresentation last)
        {
            _location = location;
            _transition = transition;
            _first = first;
            _last = last;

            transition.Init(first, last);
        }

        public void Dispose()
        {
            _first?.Dispose();
            _last?.Dispose();

            _transition.Dispose();
        }
    }
}