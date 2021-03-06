﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Time;
using Vkm.Common;
using Vkm.Library.Common;

namespace Vkm.Library.Timer
{
    class CountdownElement : ElementBase
    {
        private TimeSpan _originalSpan;
        private readonly Stopwatch _stopwatch;

        private ITimerToken _elapsedToken;

        private byte _hours = 99;
        private byte _minutes = 99;
        private byte _seconds = 99;

        private bool _timeRequested;


        public override DeviceSize ButtonCount => new DeviceSize(5, 1);

        public CountdownElement(Identifier identifier) : base(identifier)
        {
            _stopwatch = new Stopwatch();
        }

        public override void Init()
        {
            base.Init();

            RegisterTimer(new TimeSpan(0, 0, 0, 0, 500), DrawStopwatch);

        }

        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            if (_timeRequested && previousLayout is InputTimeLayout itl)
            {
                _originalSpan = new TimeSpan(0, 0, itl.Values[0] * 10 + itl.Values[1], itl.Values[2] * 10 + itl.Values[3]);
                _timeRequested = false;
            }

            DrawCommon();

            DrawStopwatch();
        }

        protected override void OnLeavingLayout()
        {
            _hours = 99;
            _minutes = 99;
            _seconds = 99;
        }

        private void DrawCommon()
        {
            var bmp1 = LayoutContext.CreateBitmap();
            DefaultDrawingAlgs.DrawText(bmp1, FontService.Instance.AwesomeFontFamily, FontAwesomeRes.fa_undo, GlobalContext.Options.Theme.ForegroundColor);
         
            var bmp2 = LayoutContext.CreateBitmap();
            DefaultDrawingAlgs.DrawText(bmp2, FontService.Instance.AwesomeFontFamily, FontAwesomeRes.fa_play, GlobalContext.Options.Theme.ForegroundColor);

            DrawInvoke(new[] {new LayoutDrawElement(new Location(3, 0), bmp1), new LayoutDrawElement(new Location(4, 0), bmp2)});
        }


        void DrawStopwatch()
        {
            DrawInvoke(ProvideTimer());
        }

        IEnumerable<LayoutDrawElement> ProvideTimer()
        {
            var elapsed = _originalSpan - _stopwatch.Elapsed;

            if (_hours != elapsed.Hours)
            {
                _hours = (byte)elapsed.Hours;
                yield return new LayoutDrawElement(new Location(0, 0), DrawNumber(_hours));
            }
            
            if (_minutes != elapsed.Minutes)
            {
                _minutes = (byte)elapsed.Minutes;
                yield return new LayoutDrawElement(new Location(1, 0), DrawNumber(_minutes));
            }

            if (_seconds != elapsed.Seconds)
            {
                _seconds = (byte)elapsed.Seconds;
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
                if (location.X == 3)
                {
                    _stopwatch.Reset();
                    Stop();

                }
                else if (location.X == 4)
                {
                    if (_stopwatch.IsRunning)
                        Stop();
                    else
                        Start();
                }
                else
                {
                    _timeRequested = true;
                    LayoutContext.SetLayout(GlobalContext.InitializeEntity(new InputTimeLayout(new Identifier(""))));
                }
            }

            return base.ButtonPressed(location, buttonEvent);
        }

        void Stop()
        {
            _stopwatch.Stop();
            _elapsedToken?.Stop();
            _elapsedToken = null;
            DrawStopwatch();
        }

        void Start()
        {
            var span = _originalSpan - _stopwatch.Elapsed;
            if (span.TotalMilliseconds > 0)
            {
                _stopwatch.Start();
                _elapsedToken = GlobalContext.Services.TimerService.RegisterTimer(span, Finish);
                _elapsedToken.Start();
            }
        }

        void Finish()
        {
            Stop();
            _stopwatch.Reset();
            LayoutContext.SetLayout(GlobalContext.InitializeEntity(new TimerElapsedLayout(Id)));
        }
    }
}
