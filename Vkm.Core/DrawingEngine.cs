using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Device;
using Vkm.Api.Layout;

namespace Vkm.Core
{
    class DrawingEngine: IDisposable
    {
        private readonly IDevice _device;
        private readonly LayoutContext _layoutContext;

        private readonly ConcurrentDictionary<Location, LayoutDrawElement> _imagesToDevice;
        private readonly VisualEffectProcessor _visualEffectProcessor;

        private int _counter;

        public DrawingEngine(IDevice device, LayoutContext layoutContext)
        {
            _device = device;
            _layoutContext = layoutContext;

            _imagesToDevice = new ConcurrentDictionary<Location, LayoutDrawElement>();
            _visualEffectProcessor = new VisualEffectProcessor(_device);
        }

        public IDisposable PauseDrawing()
        {
            return new Token(this);
        }

        public void DrawBitmap(LayoutDrawElement drawElement)
        {
            _imagesToDevice.AddOrUpdate(drawElement.Location, drawElement, (l, b) =>
            {
                b.BitmapRepresentation.Dispose();

                return drawElement;
            });

            if (_counter == 0)
                PerformDraw();
        }

        public void ClearDevice()
        {
            for (byte i = 0; i < _device.ButtonCount.Width; i++)
                for (byte j = 0; j < _device.ButtonCount.Height; j++)
                {
                    var location = new Location(i, j);
                    var drawElement = new LayoutDrawElement(location, _layoutContext.CreateBitmap());
                    DrawBitmap(drawElement);
                }
        }

        private void PerformDraw()
        {
            List<LayoutDrawElement> currentDrawElementsCache = new List<LayoutDrawElement>();

            var keys = _imagesToDevice.Keys;
            foreach (var location in keys)
            {
                if (_imagesToDevice.TryRemove(location, out var drawElement))
                {
                    currentDrawElementsCache.Add(drawElement);
                }
            }
            
            _visualEffectProcessor.Draw(currentDrawElementsCache);
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

        public void Dispose()
        {
            _visualEffectProcessor.Dispose();
        }
    }
}