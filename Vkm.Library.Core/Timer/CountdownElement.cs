﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Drawable;
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
        private readonly ClockDrawer _clockDrawer = new ClockDrawer();
        
        private TimeSpan _originalSpan;
        private readonly Stopwatch _stopwatch;

        private ITimerToken _elapsedToken;

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
            _clockDrawer.Reset();
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

            return _clockDrawer.ProvideDrawElements((byte) elapsed.Hours, (byte) elapsed.Minutes, (byte) elapsed.Seconds, GlobalContext, LayoutContext);
        }

        public override void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
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
