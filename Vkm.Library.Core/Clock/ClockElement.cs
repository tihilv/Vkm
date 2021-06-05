using System;
using System.Collections.Generic;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Drawable;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Library.Common;

namespace Vkm.Library.Clock
{
    internal class ClockElement: ElementBase, IOptionsProvider
    {
        private ClockElementOptions _options;

        private readonly ClockDrawer _clockDrawer = new ClockDrawer();

        public override DeviceSize ButtonCount => new DeviceSize(3, 1);

        
        public ClockElement(Identifier id) : base(id)
        {
            
        }

        public IOptions GetDefaultOptions()
        {
            return new ClockElementOptions();
        }

        public void InitOptions(IOptions options)
        {
            _options = (ClockElementOptions) options;
        }

        public override void Init()
        {
            base.Init();
            RegisterTimer(new TimeSpan(0,0,0,1), () =>  DrawInvoke(ProvideTime()));
        }

        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            DrawInvoke(ProvideTime());
        }

        protected override void OnLeavedLayout()
        {
            _clockDrawer.Reset();
        }

        IEnumerable<LayoutDrawElement> ProvideTime()
        {
            var now = DateTime.Now;
            return _clockDrawer.ProvideDrawElements((byte) now.Hour, (byte) now.Minute, (byte) now.Second, GlobalContext, LayoutContext);
        }

        public override void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
        {
            if (buttonEvent == ButtonEvent.Down)
                LayoutContext.SetLayout(_options.TimerLayoutIdentifier);
        }
    }
}
