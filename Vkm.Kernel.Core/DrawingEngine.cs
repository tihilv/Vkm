using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Vkm.Api.Basic;
using Vkm.Api.Common;
using Vkm.Api.Device;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Api.Transition;
using Vkm.Kernel.VisualEffect;

namespace Vkm.Kernel
{
    class DrawingEngine: IDisposable
    {
        private IDevice _device;
        private readonly ThemeOptions _themeOptions;

        private readonly LazyDictionary<Location, LayoutDrawElement> _imagesToDevice;

        private readonly ConcurrentDictionary<Location, Location> _switchedLocations;

        private VisualEffectProcessor _visualEffectProcessor;

        private int _counter;

        private byte _brightness;
        private byte _brightnessSet;

        internal bool IsAllDrawn => _visualEffectProcessor.IsAllDrawn;
        
        public DrawingEngine(IDevice device, ThemeOptions themeOptions, IVisualTransitionFactory visualTransitionFactory)
        {
            _device = device;
            _themeOptions = themeOptions;

            _imagesToDevice = new LazyDictionary<Location, LayoutDrawElement>();
            _switchedLocations = new ConcurrentDictionary<Location, Location>();
            _visualEffectProcessor = new VisualEffectProcessor(_device, visualTransitionFactory);
        }

        public IDisposable PauseDrawing()
        {
            return new Token(this);
        }

        public void DrawBitmap(LayoutDrawElement drawElement)
        {
            var value = _imagesToDevice.AddOrUpdate(drawElement.Location, (l) => drawElement, (l, b) =>
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

                    var de = drawElement.Value;
                    if (newSwitch)
                    {
                        de = new LayoutDrawElement(de.Location, de.BitmapRepresentation, new TransitionInfo(TransitionType.LayoutChange, de.TransitionInfo.Duration));
                    }

                    currentDrawElementsCache.Add(de);
                }
            }
            
            _visualEffectProcessor.Draw(currentDrawElementsCache);
        }

        public byte Brightness
        {
            get { return _brightness; }
            set
            {
                _brightness = value;
                if (_counter == 0)
                    SetBrightnessIfNeeded();
            }
        }

        void SetBrightnessIfNeeded()
        {
            if (_brightness != _brightnessSet)
            {
                _brightnessSet = _brightness;
                _device.SetBrightness(_brightness);
            }
        }

        public void Dispose()
        {
            DisposeHelper.DisposeAndNull(ref _visualEffectProcessor);
            DisposeHelper.DisposeAndNull(ref _device);
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
                {
                    _drawingEngine.SetBrightnessIfNeeded();
                    _drawingEngine.PerformDraw();
                }
            }
        }
    }
}