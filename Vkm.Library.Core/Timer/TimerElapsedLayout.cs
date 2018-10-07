using System;
using System.Collections.Generic;
using System.Drawing;
using Vkm.Api.Basic;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Transition;

namespace Vkm.Library.Timer
{
    class TimerElapsedLayout: LayoutBase
    {
        private TimeSpan _frameDuration = TimeSpan.FromSeconds(1);
        readonly Random _random = new Random();

        public TimerElapsedLayout(Identifier identifier) : base(identifier)
        {
        }

        public override void Init()
        {
            base.Init();

            RegisterTimer(_frameDuration, Draw);
        }

        private void Draw()
        {
            List<LayoutDrawElement> result = new List<LayoutDrawElement>();
            for (byte i = 0; i < LayoutContext.ButtonCount.Width; i++)
            for (byte j = 0; j < LayoutContext.ButtonCount.Height; j++)
            {
                var bmp = LayoutContext.CreateBitmap();

                using (var grahics = bmp.CreateGraphics())
                using (var brush = new SolidBrush(Color.FromArgb(_random.Next(0, 255), _random.Next(0, 255), _random.Next(0, 255), _random.Next(0, 255))))
                {
                    grahics.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);
                }

                result.Add(new LayoutDrawElement(new Location(i, j), bmp, new TransitionInfo(TransitionType.ElementUpdate, _frameDuration)));
            }

            DrawInvoke(result);
        }

        public override void ButtonPressed(Location location, bool isDown)
        {
            base.ButtonPressed(location, isDown);

            LayoutContext.SetPreviousLayout();
        }
    }
}
