using System;
using System.Collections.Generic;
using System.Diagnostics;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Device;
using Vkm.Api.Layout;

namespace Vkm.Core
{
    public class DeviceManager: IDisposable
    {
        private DrawingEngine _drawingEngine;

        private readonly GlobalContext _globalContext;
        private readonly LayoutContext _layoutContext;

        private ILayout _layout;

        private readonly Stack<ILayout> _layouts;

        public DeviceManager(GlobalContext globalContext, IDevice device)
        {
            _globalContext = globalContext;
            
            _layouts = new Stack<ILayout>();

            device.ButtonEvent += DeviceOnKeyEvent;

            device.Init();
            _drawingEngine = new DrawingEngine(device, _globalContext.Options.Theme);
            _layoutContext = new LayoutContext(device, globalContext, SetLayout, SetPreviousLayout, () => _drawingEngine.PauseDrawing());

        }

        private void DeviceOnKeyEvent(object sender, ButtonEventArgs e)
        {
            using (_drawingEngine.PauseDrawing())
                _layout?.ButtonPressed(e.Location, e.IsDown);
        }

        public void SetPreviousLayout()
        {
            if (_layouts.Count >= 2)
            {
                _layouts.Pop();
                SetLayout(_layouts.Pop());
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
