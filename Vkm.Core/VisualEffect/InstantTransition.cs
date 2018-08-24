using Vkm.Api.Basic;
using Vkm.Api.VisualEffect;

namespace Vkm.Core
{
    internal class InstantTransition : IVisualTransition
    {
        private bool _first = true;
        private BitmapRepresentation _current;

        public BitmapRepresentation Current => _current;

        public bool HasNext => _first;

        public void Init(BitmapRepresentation first, BitmapRepresentation last, int steps)
        {
            _current = last.Clone();
        }

        public void ReplaceLastBitmap(BitmapRepresentation last)
        {
            DisposeHelper.DisposeAndNull(ref _current);
            _current = last.Clone();
        }

        public void Next()
        {
            _first = false;
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndNull(ref _current);
        }

    }
}