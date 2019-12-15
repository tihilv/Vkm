using System;
using System.Collections.Generic;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Common;

namespace Vkm.Library.Clock
{
    internal class ClockElement: ElementBase, IOptionsProvider
    {
        private ClockElementOptions _options;

        public override DeviceSize ButtonCount => new DeviceSize(3, 1);

        private byte _hours = 99;
        private byte _minutes = 99;
        private byte _seconds = 99;
        
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
            _hours = 99;
            _minutes = 99;
            _seconds = 99;
        }

        IEnumerable<LayoutDrawElement> ProvideTime()
        {
            var now = DateTime.Now;

            if (_hours != now.Hour)
            {
                _hours = (byte)now.Hour;
                yield return new LayoutDrawElement(new Location(0, 0), DrawNumber(_hours));
            }

            if (_minutes != now.Minute)
            {
                _minutes = (byte)now.Minute;
                yield return new LayoutDrawElement(new Location(1, 0), DrawNumber(_minutes));
            }

            if (_seconds != now.Second)
            {
                _seconds = (byte)now.Second;
                yield return new LayoutDrawElement(new Location(2, 0), DrawNumber(_seconds));
            }
        }

        private BitmapEx DrawNumber(byte number)
        {
            var bitmap = LayoutContext.CreateBitmap();

            var fontFamily = GlobalContext.Options.Theme.FontFamily;

            var str = number.ToString("00");
            DefaultDrawingAlgs.DrawText(bitmap, fontFamily, str, GlobalContext.Options.Theme.ForegroundColor);

            return bitmap;
        }

        public override bool ButtonPressed(Location location, ButtonEvent buttonEvent)
        {
            if (buttonEvent == ButtonEvent.Down)
            {
                LayoutContext.SetLayout(_options.TimerLayoutIdentifier);
            }

            return base.ButtonPressed(location, buttonEvent);
        }
    }
}
