using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using Vkm.Api.Basic;
using Vkm.Api.VisualEffect;

namespace Vkm.Library.VisualTransition
{
    internal class FadeTransition : IVisualTransition
    {
        private static readonly ConcurrentDictionary<float, Lazy<ImageAttributes>> _transformData = new ConcurrentDictionary<float, Lazy<ImageAttributes>>();
        
        private int _steps;

        private int _currentStep;

        private BitmapEx _firstBitmap;
        private BitmapEx _lastBitmap;
        private BitmapEx _currentBitmap;

        private BitmapRepresentation _current;

        public BitmapRepresentation Current
        {
            get => _current;
            private set
            {
                DisposeHelper.DisposeAndNull(ref _current);
                _current = value;
            }
        }

        public bool HasNext => _currentStep < _steps;

        private static ImageAttributes GetTransformData(float value)
        {
            return _transformData.GetOrAdd(value, f => new Lazy<ImageAttributes>(()=>
            {
                ColorMatrix colorMatrix = new ColorMatrix {Matrix33 = f};
                ImageAttributes imgAttribute = new ImageAttributes();
                imgAttribute.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                return imgAttribute;
                
            })).Value;
        }

        public void Init(BitmapRepresentation first, BitmapRepresentation last, int steps)
        {
            _steps = steps;

            Current = first.Clone();

            _firstBitmap = first.CreateBitmap();
            _lastBitmap = last.CreateBitmap();
            _currentBitmap = first.CreateBitmap();
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
                using (Graphics graphics = _currentBitmap.CreateGraphics())
                {
                    graphics.DrawImage(_firstBitmap.GetInternal(), new Rectangle(0, 0, _currentBitmap.Width, _currentBitmap.Height), 0, 0, _currentBitmap.Width, _currentBitmap.Height, GraphicsUnit.Pixel);

                    var transformData = GetTransformData((float) _currentStep / _steps);
                    graphics.DrawImage(_lastBitmap.GetInternal(), new Rectangle(0, 0, _lastBitmap.Width, _lastBitmap.Height), 0, 0, _lastBitmap.Width, _lastBitmap.Height, GraphicsUnit.Pixel, transformData);

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
}