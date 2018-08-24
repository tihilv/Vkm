﻿using System;
using Vkm.Api.Basic;
using Vkm.Api.Device;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;

namespace Vkm.Api.Data
{
    public class LayoutContext
    {
        private readonly Action<ILayout> _setLayoutAction;
        private readonly Action _setPreviousLayoutAction;
        private Func<IDisposable> _pauseDrawingFunc;
        private readonly GlobalContext _globalContext;
        private readonly IDevice _device;
        
        public IconSize IconSize { get; private set; }
        public DeviceSize ButtonCount { get; private set; }

        public GlobalOptions Options => _globalContext.Options;

        public LayoutContext(IDevice device, GlobalContext globalContext, Action<ILayout> setLayoutAction, Action setPreviousLayoutAction, Func<IDisposable> pauseDrawingFunc)
        {
            _device = device;
            IconSize = device.IconSize;
            ButtonCount = device.ButtonCount;
            _globalContext = globalContext;
            _pauseDrawingFunc = pauseDrawingFunc;

            _setLayoutAction = setLayoutAction;
            _setPreviousLayoutAction = setPreviousLayoutAction;
        }

        public BitmapEx CreateBitmap()
        {
            return _device.CreateBitmap(Options.Theme);
        }

        public void SetLayout(Identifier layoutIdentifier)
        {
            _setLayoutAction(_globalContext.Layouts[layoutIdentifier]);
        }

        public void SetLayout(ILayout layout)
        {
            _setLayoutAction(layout);
        }

        public void SetPreviousLayout()
        {
            _setPreviousLayoutAction();
        }

        public IDisposable PauseDrawing()
        {
            return _pauseDrawingFunc();
        }
    }
}
