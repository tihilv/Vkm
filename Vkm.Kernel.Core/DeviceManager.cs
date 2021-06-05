using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Common;
using Vkm.Api.Data;
using Vkm.Api.Device;
using Vkm.Api.Drawable;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Time;
using Vkm.Api.Transition;

namespace Vkm.Kernel
{
    public class DeviceManager: IDisposable
    {
        private DrawingEngine _drawingEngine;

        private readonly GlobalContext _globalContext;
        private readonly LayoutContext _layoutContext;

        private ILayout _layout;

        private readonly Stack<ILayout> _layouts;

        private readonly LazyDictionary<Location, ITimerToken> _pressedButtons;

        public bool IsAllDrawn => _drawingEngine.IsAllDrawn;
        
        public DeviceManager(GlobalContext globalContext, IDevice device)
        {
            _globalContext = globalContext;
            _globalContext.LayoutRemoved += OnLayoutRemoved;
            
            _layouts = new Stack<ILayout>();
            _pressedButtons = new LazyDictionary<Location, ITimerToken>();

            device.ButtonEvent += DeviceOnKeyEvent;

            device.Init();
            _drawingEngine = new DrawingEngine(device, _globalContext.Options.Theme, globalContext.Services.ModulesService.GetModules<IVisualTransitionFactory>().First());
            _layoutContext = new LayoutContext(device, globalContext, SetLayout, () => SetPreviousLayout(), () => _drawingEngine.PauseDrawing(), GetAvailableLayouts);

        }

        private void DeviceOnKeyEvent(object sender, ButtonEventArgs e)
        {
            if (e.IsDown)
            {
                var value = _pressedButtons.AddOrUpdate(e.Location, l =>
                    {
                        var result = _globalContext.Services.TimerService.RegisterTimer(_globalContext.Options.LongPressTimeout, () =>
                        {
                            DoButtonPressed(e.Location, ButtonEvent.LongPress);

                            _pressedButtons.TryRemove(l, out _);
                        }, true);

                        result.Start();

                        return result;
                    },
                    null);
            }
            else
            {
                if (_pressedButtons.TryRemove(e.Location, out var token))
                    token.Value.Stop();
            }
            DoButtonPressed(e.Location, e.IsDown?ButtonEvent.Down:ButtonEvent.Up);
        }

        void DoButtonPressed(Location location, ButtonEvent buttonEvent)
        {
            using (_drawingEngine.PauseDrawing())
                if (_globalContext.DeviceHooks.Values.All(h => !h.OnKeyEventHook(location, buttonEvent, _layout, _layoutContext)))
                    _layout?.ButtonPressed(location, buttonEvent, _layoutContext);
        }

        private void ClearPressedButtons()
        {
            var keys = _pressedButtons.Keys.ToArray();
            foreach (var location in keys)
            {
                if (_pressedButtons.TryRemove(location, out var token))
                    token.Value.Stop();
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
                } else if (_layouts.Count == 1)
                {
                    _layouts.Pop();
                    SetLayout(null);
                }
            }
        }

        private IEnumerable<ILayout> GetAvailableLayouts()
        {
            return _layouts.Union(_globalContext.Layouts.Values).Distinct();
        }
        
        private void OnLayoutRemoved(object sender, EventArgs e)
        {
            ILayout layout = (ILayout) sender;
            
            if (_layouts.Peek() == layout)
                SetPreviousLayout(layout.Id);
            else if (_layouts.Contains(layout))
            {
                Stack<ILayout> temp = new Stack<ILayout>();
                do
                {
                    temp.Push(_layouts.Pop());
                } while (temp.Peek() != layout);

                temp.Pop();
                while (temp.Any())
                    _layouts.Push(temp.Pop());
            }
        }
        
        public void SetLayout(ILayout layout)
        {
            using (_drawingEngine.PauseDrawing())
            {
                if (layout != null)
                {
                    if (_layouts.Contains(layout))
                        while (_layouts.Peek() != layout)
                            _layouts.Pop();
                    else
                        _layouts.Push(layout);
                }

                if (layout != _layout)
                {
                    ClearPressedButtons();

                    var oldLayout = _layout;

                    if (oldLayout != null)
                    {
                        oldLayout.LeaveLayout(); // can provide info to draw
                        oldLayout.DrawRequested -= OnDrawRequested;
                    }

                    _drawingEngine.ClearDevice();
                    _layout = layout;

                    if (_layout != null)
                    {
                        _layout.DrawRequested += OnDrawRequested;
                        _drawingEngine.Brightness = _layout.PreferredBrightness ?? _globalContext.Options.Brightness;

                        _layout.EnterLayout(_layoutContext, oldLayout);
                    }
                }
            }
        }

        private void OnDrawRequested(object sender, DrawEventArgs e)
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
            _globalContext.LayoutRemoved -= OnLayoutRemoved;
            
            _drawingEngine.Brightness = 0;
            SetLayout(null);
            DisposeHelper.DisposeAndNull(ref _drawingEngine);
        }
    }
}
