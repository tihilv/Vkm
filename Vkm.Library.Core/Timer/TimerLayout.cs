using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Drawable;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Library.Common;

namespace Vkm.Library.Timer
{
    class TimerLayout: LayoutBase
    {
        private const int TimersCount = 4;

        private short _currentPage;

        private readonly CountdownElement[] _countdownElements;
        private readonly StopwatchElement[] _stopwatchElements;
        
        public TimerLayout(Identifier id) : base(id)
        {
            _countdownElements = new CountdownElement[TimersCount];
            _stopwatchElements = new StopwatchElement[TimersCount];

            _currentPage = -1;
        }

        public override void Init()
        {
            base.Init();

            for (int i = 0; i < TimersCount; i++)
            {
                _countdownElements[i] = GlobalContext.InitializeEntity(new CountdownElement(new Identifier($"Vkm.Countdown.N{i}")));
                _stopwatchElements[i] = GlobalContext.InitializeEntity(new StopwatchElement(new Identifier($"Vkm.Stopwatch.N{i}")));
            }

            AddElement(new Location(TimersCount, 2), GlobalContext.InitializeEntity(new BackElement()));
        }

        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            if (_currentPage == -1)
                ReplaceTimers(0, layoutContext);
            else
                DrawMarkers(layoutContext);
        }

        void ReplaceTimers(byte toPage, LayoutContext layoutContext)
        {
            _currentPage = toPage;
            var countdownLocation = new Location(0, 0);
            var timerLocation = new Location(0, 1);

            if (Elements.Count() > 1)
            {
                RemoveElement(countdownLocation);
                RemoveElement(timerLocation);
            }

            AddElement(countdownLocation, _countdownElements[toPage]);
            AddElement(timerLocation, _stopwatchElements[toPage]);

            DrawMarkers(layoutContext);
        }

        private void DrawMarkers(LayoutContext layoutContext)
        {
            List<LayoutDrawElement> result = new List<LayoutDrawElement>();

            for (byte i = 0; i < TimersCount; i++)
            {
                var bmp = layoutContext.CreateBitmap();

                using (var graphics = bmp.CreateGraphics())
                using (var brush = new SolidBrush(GlobalContext.Options.Theme.ForegroundColor))
                {
                    int r = 10;
                    if (i == _currentPage)
                        r = 25;
                    var radius = Math.Min(bmp.Width, bmp.Height) * r / 100;
                    graphics.FillEllipse(brush, (bmp.Width - radius) / 2, (bmp.Width - radius) / 2, radius, radius);
                }

                result.Add(new LayoutDrawElement(new Location(i, 2), bmp));
            }

            DrawInvoke(result);
        }

        public override void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
        {
            if (buttonEvent == ButtonEvent.Down)
            {
                if (location.Y == 2 && location.X < TimersCount)
                {
                    ReplaceTimers(location.X, layoutContext);
                }
            }

            base.ButtonPressed(location, buttonEvent, layoutContext);
        }

        

        
    }
}
