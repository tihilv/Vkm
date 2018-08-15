using System;
using System.Collections.Generic;
using Vkm.Api.Data;
using Vkm.Api.Device;
using Vkm.Api.Layout;

namespace Vkm.Core
{
    public class DeviceManager: IDisposable
    {
        private readonly IDevice _device;
        private readonly GlobalContext _globalContext;

        private readonly LayoutContext _layoutContext;

        private ILayout _layout;

        private readonly Stack<ILayout> _layouts;

        public DeviceManager(GlobalContext globalContext, IDevice device)
        {
            _globalContext = globalContext;
            _device = device;

            _layouts = new Stack<ILayout>();

            _device.ButtonEvent += DeviceOnKeyEvent;

            _device.Init();
            _layoutContext = new LayoutContext(_device.ButtonCount, _device.IconSize, globalContext, SetLayout, SetPreviousLayout);
        }

        private void DeviceOnKeyEvent(object sender, ButtonEventArgs e)
        {
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
            if (_layouts.Contains(layout))
                while (_layouts.Peek() != layout)
                    _layouts.Pop();
            else
                _layouts.Push(layout);

            if (layout != _layout)
            {
                if (_layout != null)
                {
                    _layout.LeaveLayout();
                    _layout.DrawLayout -= LayoutOnDrawLayout;
                }

                _device.Clear();

                _layout = layout;

                if (_layout != null)
                {
                    _layout.DrawLayout += LayoutOnDrawLayout;

                    _device.SetBrightness(_layout.PreferredBrightness ?? _globalContext.Options.Brightness);

                    _layout.EnterLayout(_layoutContext);
                }
            }
        }

        private void LayoutOnDrawLayout(object sender, DrawEventArgs e)
        {
            foreach (var element in e.Elements)
            {
                if (element.Bitmap != null)
                {
                    _device.SetBitmap(element.Location, element.Bitmap);
                    element.Bitmap.Dispose();
                }
            }
        }

        public void Dispose()
        {
            SetLayout(null);
            _device?.Dispose();
        }
    }
}
