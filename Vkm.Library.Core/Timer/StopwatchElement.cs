using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;
using Vkm.Library.Common;

namespace Vkm.Library.Timer
{
    class StopwatchElement: ElementBase
    {
        private readonly Stopwatch _stopwatch;

        private byte _minutes = 99;
        private byte _seconds = 99;
        private byte _secondsPart = 99;

        
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

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            DrawCommon();

            DrawStopwatch();
        }

        public override void LeaveLayout()
        {
            _minutes = 99;
            _seconds = 99;
            _secondsPart = 99;

            base.LeaveLayout();
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

            if (_minutes != elapsed.Minutes)
            {
                _minutes = (byte)elapsed.Minutes;
                yield return new LayoutDrawElement(new Location(0, 0), DrawNumber(_minutes));
            }

            if (_seconds != elapsed.Seconds)
            {
                _seconds = (byte)elapsed.Seconds;
                yield return new LayoutDrawElement(new Location(1, 0), DrawNumber(_seconds));
            }

            var secondsPart = (byte)(60*elapsed.Milliseconds/1000);
            if (_secondsPart != secondsPart)
            {
                _secondsPart = secondsPart;
                yield return new LayoutDrawElement(new Location(2, 0), DrawNumber(_secondsPart));
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
            else if (location.X == 4)
            {
                _stopwatch.Stop();
                DrawStopwatch();
            }

            return base.ButtonPressed(location, buttonEvent);
        }
    }
}
