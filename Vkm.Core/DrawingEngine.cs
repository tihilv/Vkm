using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Device;

namespace Vkm.Core
{
    class DrawingEngine
    {
        private readonly IDevice _device;
        private readonly LayoutContext _layoutContext;

        private readonly ConcurrentDictionary<Location, Bitmap> _imagesToDevice;

        private int _counter;

        public DrawingEngine(IDevice device, LayoutContext layoutContext)
        {
            _device = device;
            _layoutContext = layoutContext;

            _imagesToDevice = new ConcurrentDictionary<Location, Bitmap>();
        }

        public IDisposable PauseDrawing()
        {
            return new Token(this);
        }

        public void DrawBitmap(Location location, Bitmap bitmap)
        {
            _imagesToDevice.AddOrUpdate(location, bitmap, (l, b) =>
            {
                b.Dispose();

                return bitmap;
            });

            if (_counter == 0)
                PerformDraw();
        }

        public void ClearDevice()
        {
            for (byte i = 0; i < _device.ButtonCount.Width; i++)
            for (byte j = 0; j < _device.ButtonCount.Height; j++)
                DrawBitmap(new Location(i, j), _layoutContext.CreateBitmap());
        }

        private void PerformDraw()
        {
            var keys = _imagesToDevice.Keys;
            foreach (var location in keys)
            {
                if (_imagesToDevice.TryRemove(location, out var bitmap))
                {
                    _device.SetBitmap(location, bitmap);
                    bitmap.Dispose();
                }
            }
        }

        class Token : IDisposable
        {
            private readonly DrawingEngine _drawingEngine;

            public Token(DrawingEngine drawingEngine)
            {
                _drawingEngine = drawingEngine;
                Interlocked.Increment(ref _drawingEngine._counter);
            }

            public void Dispose()
            {
                if (Interlocked.Decrement(ref _drawingEngine._counter) == 0)
                    _drawingEngine.PerformDraw();
            }
        }
    }
}