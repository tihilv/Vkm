using System.Drawing;
using Vkm.Api.Basic;
using Vkm.Api.VisualEffect;

namespace Vkm.Kernel.VisualEffect
{
    internal class MoveTransition : IVisualTransition
    {
        private MoveFromDirection _moveFromDirection = MoveFromDirection.Right;

        private int _steps;

        private int _currentStep;

        private BitmapEx _firstBitmap;
        private BitmapEx _lastBitmap;
        private BitmapEx _currentBitmap;

        private BitmapRepresentation _current;

        private float _currentX;
        private float _currentY;
        private float _stepX;
        private float _stepY;

        public BitmapRepresentation Current
        {
            get => _current;
            set
            {
                DisposeHelper.DisposeAndNull(ref _current);
                _current = value;
            }
        }

        public bool HasNext => _currentStep < _steps;

        public void Init(BitmapRepresentation first, BitmapRepresentation last, int steps)
        {
            _steps = steps;

            Current = first.Clone();

            _firstBitmap = first.CreateBitmap();
            _lastBitmap = last.CreateBitmap();
            _currentBitmap = first.CreateBitmap();

            switch (_moveFromDirection)
            {
                case MoveFromDirection.Bottom:
                    _currentX = 0;
                    _currentY = _currentBitmap.Height;
                    _stepX = 0;
                    _stepY = -_currentBitmap.Height / (float) steps;
                    break;
                
                case MoveFromDirection.Top:
                    _currentX = 0;
                    _currentY = -_currentBitmap.Height;
                    _stepX = 0;
                    _stepY = _currentBitmap.Height / (float) steps;
                    break;

                case MoveFromDirection.Left:
                    _currentX = -_currentBitmap.Width;
                    _currentY = 0;
                    _stepX = _currentBitmap.Width / (float) steps;
                    _stepY = 0;
                    break;

                case MoveFromDirection.Right:
                    _currentX = _currentBitmap.Width;
                    _currentY = 0;
                    _stepX = -_currentBitmap.Width / (float) steps;
                    _stepY = 0;
                    break;
            }

            _currentX += _stepX;
            _currentY += _stepY;
        }

        public void ReplaceLastBitmap(BitmapRepresentation last)
        {
            DisposeHelper.DisposeAndNull(ref _lastBitmap);
            _lastBitmap = last.CreateBitmap();
        }

        public void Next()
        {
            if (HasNext)
            {
                _currentStep++;

                if (!HasNext)
                {
                    _currentX = 0;
                    _currentY = 0;
                }
                else
                {
                    _currentX += _stepX;
                    _currentY += _stepY;
                }

                using (Graphics graphics = _currentBitmap.CreateGraphics())
                {
                    graphics.DrawImage(_firstBitmap.GetInternal(), new Rectangle(0, 0, _currentBitmap.Width, _currentBitmap.Height), 0, 0, _currentBitmap.Width, _currentBitmap.Height, GraphicsUnit.Pixel);

                    graphics.DrawImage(_lastBitmap.GetInternal(), new Rectangle((int)_currentX, (int)_currentY, _lastBitmap.Width, _lastBitmap.Height), 0, 0, _lastBitmap.Width, _lastBitmap.Height, GraphicsUnit.Pixel);

                    Current = new BitmapRepresentation(_currentBitmap);
                }
            }
        }

        public void Dispose()
        {
            Current = null;

            DisposeHelper.DisposeAndNull(ref _firstBitmap);
            DisposeHelper.DisposeAndNull(ref _lastBitmap);
            DisposeHelper.DisposeAndNull(ref _currentBitmap);
        }
    }

    public enum MoveFromDirection
    {
        Left, Right, Top, Bottom
    }
}