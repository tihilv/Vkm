using System;
using System.Collections.Generic;
using System.Diagnostics;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Drawable;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;
using Vkm.Library.Common;

namespace Vkm.Library.Timer
{
    class StopwatchElement: ElementBase
    {
        private readonly ClockDrawer _clockDrawer = new ClockDrawer();
        
        private readonly Stopwatch _stopwatch;

        public override DeviceSize ButtonCount => new DeviceSize(5, 1);


        public StopwatchElement(Identifier identifier) : base(identifier)
        {
            _stopwatch = new Stopwatch();

        }

        public override void Init()
        {
            base.Init();

            RegisterTimer(new TimeSpan(0, 0, 0, 0, 50), DrawStopwatch);

        }

        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            DrawCommon();

            DrawStopwatch();
        }

        protected override void OnLeavingLayout()
        {
            _clockDrawer.Reset();
        }

        private void DrawCommon()
        {
            var bmp1 = LayoutContext.CreateBitmap();
            DefaultDrawingAlgs.DrawText(bmp1, FontService.Instance.AwesomeFontFamily, FontAwesomeRes.fa_undo, GlobalContext.Options.Theme.ForegroundColor);
         
            var bmp2 = LayoutContext.CreateBitmap();
            DefaultDrawingAlgs.DrawText(bmp2, FontService.Instance.AwesomeFontFamily, FontAwesomeRes.fa_hand_grab_o, GlobalContext.Options.Theme.ForegroundColor);

            DrawInvoke(new[] {new LayoutDrawElement(new Location(3, 0), bmp1), new LayoutDrawElement(new Location(4, 0), bmp2)});
        }


        void DrawStopwatch()
        {
            DrawInvoke(ProvideTimer());
        }

        IEnumerable<LayoutDrawElement> ProvideTimer()
        {
            var elapsed = _stopwatch.Elapsed;
            var secondsPart = (byte)(60*elapsed.Milliseconds/1000);

            return _clockDrawer.ProvideDrawElements((byte) elapsed.Minutes, (byte) elapsed.Seconds, secondsPart, GlobalContext, LayoutContext);
        }

        public override void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
        {
            if (buttonEvent == ButtonEvent.Down)
            {
                if (location.X == 3)
                {
                    _stopwatch.Reset();
                    DrawStopwatch();
                }
                else
                {
                    if (_stopwatch.IsRunning)
                    {
                        _stopwatch.Stop();
                        DrawStopwatch();
                    }
                    else
                        _stopwatch.Start();
                }
            }
            else if (location.X == 4 &&  buttonEvent != ButtonEvent.LongPress)
            {
                _stopwatch.Stop();
                DrawStopwatch();
            }
        }
    }
}
