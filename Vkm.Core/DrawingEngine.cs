using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Vkm.Api.Basic;
using Vkm.Api.Device;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Api.Transition;

namespace Vkm.Core
{
    class DrawingEngine: IDisposable
    {
        private readonly IDevice _device;
        private readonly ThemeOptions _themeOptions;

        private readonly ConcurrentDictionary<Location, LayoutDrawElement> _imagesToDevice;

        private readonly ConcurrentDictionary<Location, Location> _switchedLocations;

        private VisualEffectProcessor _visualEffectProcessor;

        private int _counter;

        public DrawingEngine(IDevice device, ThemeOptions themeOptions)
        {
            _device = device;
            _themeOptions = themeOptions;

            _imagesToDevice = new ConcurrentDictionary<Location, LayoutDrawElement>();
            _switchedLocations = new ConcurrentDictionary<Location, Location>();
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
            _switchedLocations.Clear();

            for (byte i = 0; i < _device.ButtonCount.Width; i++)
                for (byte j = 0; j < _device.ButtonCount.Height; j++)
                {
                    var location = new Location(i, j);
                    var drawElement = new LayoutDrawElement(location, _device.CreateBitmap(_themeOptions));
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
                    bool newSwitch = false;
                    if (!_switchedLocations.ContainsKey(location))
                    {
                        newSwitch = true;
                        _switchedLocations[location] = location;
                    }

                    if (newSwitch)
                        drawElement = new LayoutDrawElement(drawElement.Location, drawElement.BitmapRepresentation, new TransitionInfo(TransitionType.LayoutChange, drawElement.TransitionInfo.Duration));

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
            DisposeHelper.DisposeAndNull(ref _visualEffectProcessor);
        }
    }
}