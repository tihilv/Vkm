using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Device;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Time;

namespace Vkm.Kernel
{
    public class DeviceManager: IDisposable
    {
        private DrawingEngine _drawingEngine;

        private readonly GlobalContext _globalContext;
        private readonly LayoutContext _layoutContext;

        private ILayout _layout;

        private readonly Stack<ILayout> _layouts;

        private readonly ConcurrentDictionary<Location, ITimerToken> _pressedButtons;

        public DeviceManager(GlobalContext globalContext, IDevice device)
        {
            _globalContext = globalContext;
            
            _layouts = new Stack<ILayout>();
            _pressedButtons = new ConcurrentDictionary<Location, ITimerToken>();

            device.ButtonEvent += DeviceOnKeyEvent;

            device.Init();
            _drawingEngine = new DrawingEngine(device, _globalContext.Options.Theme);
            _layoutContext = new LayoutContext(device, globalContext, SetLayout, () => SetPreviousLayout(), () => _drawingEngine.PauseDrawing());

        }

        private void DeviceOnKeyEvent(object sender, ButtonEventArgs e)
        {
            if (e.IsDown)
            {
                _pressedButtons.AddOrUpdate(e.Location, l =>
                    {
                        var result = _globalContext.Services.TimerService.RegisterTimer(_globalContext.Options.LongPressTimeout, () =>
                        {
                            DoButtonPressed(e.Location, ButtonEvent.LongPress);

                            _pressedButtons.TryRemove(l, out _);
                        }, true);

                        result.Start();

                        return result;
                    }, 
                    (l, d) => d);
            }
            else
            {
                if (_pressedButtons.TryRemove(e.Location, out var token))
                    token.Stop();
            }
            DoButtonPressed(e.Location, e.IsDown?ButtonEvent.Down:ButtonEvent.Up);
        }

        void DoButtonPressed(Location location, ButtonEvent buttonEvent)
        {
            using (_drawingEngine.PauseDrawing())
                if (_globalContext.DeviceHooks.Values.All(h => !h.OnKeyEventHook(location, buttonEvent, _layout)))
                    _layout?.ButtonPressed(location, buttonEvent);
        }

        private void ClearPressedButtons()
        {
            var keys = _pressedButtons.Keys.ToArray();
            foreach (var location in keys)
            {
                if (_pressedButtons.TryRemove(location, out var token))
                    token.Stop();
            }
        }

        public void SetPreviousLayout(Identifier? fromLayout = null)
        {
            if (fromLayout == null || fromLayout.Value == _layouts.Peek().Id)
            {
                if (_layouts.Count >= 2)
                {
                    _layouts.Pop();
                    SetLayout(_layouts.Pop());
                }
            }
        }

        public void SetLayout(ILayout layout)
        {
            using (_drawingEngine.PauseDrawing())
            {
                if (_layouts.Contains(layout))
                    while (_layouts.Peek() != layout)
                        _layouts.Pop();
                else
                    _layouts.Push(layout);

                if (layout != _layout)
                {
                    ClearPressedButtons();

                    var oldLayout = _layout;

                    if (oldLayout != null)
                    {
                        oldLayout.LeaveLayout(); // can provide info to draw
                        oldLayout.DrawLayout -= LayoutOnDrawLayout;
                    }

                    _drawingEngine.ClearDevice();
                    _layout = layout;

                    if (_layout != null)
                    {
                        _layout.DrawLayout += LayoutOnDrawLayout;
                        _drawingEngine.Brightness = _layout.PreferredBrightness ?? _globalContext.Options.Brightness;

                        _layout.EnterLayout(_layoutContext, oldLayout);
                    }
                }
            }
        }

        private void LayoutOnDrawLayout(object sender, DrawEventArgs e)
        {
            using (_drawingEngine.PauseDrawing())
            foreach (var element in e.Elements)
                if (element.BitmapRepresentation != null)
                    _drawingEngine.DrawBitmap(element);
        }

        private IDisposable _drawingPause;
        private byte _prevBrightness;
        public void Pause()
        {
            Debug.Assert(_drawingPause == null);

            _prevBrightness = _drawingEngine.Brightness;
            _drawingEngine.Brightness = 0;
            
            _drawingPause = _drawingEngine.PauseDrawing();
        }

        public void Resume()
        {
            Debug.Assert(_drawingPause != null);
            
            DisposeHelper.DisposeAndNull(ref _drawingPause);
            _drawingEngine.Brightness = _prevBrightness;
        }

        public void Dispose()
        {
            _drawingEngine.Brightness = 0;
            SetLayout(null);
            DisposeHelper.DisposeAndNull(ref _drawingEngine);
        }
    }
}
