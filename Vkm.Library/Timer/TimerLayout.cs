using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
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
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            DrawCommon();

            if (_currentPage == -1)
                ReplaceTimers(0);
            else
                DrawMarkers();
        }

        void ReplaceTimers(byte toPage)
        {
            _currentPage = toPage;
            var countdownLocation = new Location(0, 0);
            var timerLocation = new Location(0, 1);

            if (Elements.Any())
            {
                RemoveElement(countdownLocation);
                RemoveElement(timerLocation);
            }

            AddElement(countdownLocation, _countdownElements[toPage]);
            AddElement(timerLocation, _stopwatchElements[toPage]);

            DrawMarkers();
        }

        private void DrawMarkers()
        {
            List<LayoutDrawElement> result = new List<LayoutDrawElement>();

            for (byte i = 0; i < TimersCount; i++)
            {
                var bmp = LayoutContext.CreateBitmap();

                using (var graphics = Graphics.FromImage(bmp))
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

        private void DrawCommon()
        {
            var bmp = LayoutContext.CreateBitmap();
            DefaultDrawingAlgs.DrawText(bmp, FontService.Instance.AwesomeFontFamily, FontAwesomeRes.fa_arrow_left, FontAwesomeRes.fa_arrow_left, GlobalContext.Options.Theme.ForegroundColor);

            DrawInvoke(new[] { new LayoutDrawElement(new Location(4, 2), bmp)});
        }

        public override void ButtonPressed(Location location, bool isDown)
        {
            if (isDown)
            {
                if (location.Y == 2)
                {
                    if (location.X == 4)
                    {
                        LayoutContext.SetPreviousLayout();
                    }
                    else
                        ReplaceTimers(location.X);
                }
            }

            base.ButtonPressed(location, isDown);
        }

        

        
    }
}
