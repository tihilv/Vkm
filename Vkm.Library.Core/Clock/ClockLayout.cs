﻿using System;
using Vkm.Api.Basic;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Library.Clock
{
    internal class ClockLayout: LayoutBase
    {
        private static readonly Identifier ClockIdentifier = new Identifier("Vkm.Screensaver.Clock");

        private readonly IElement _clockElement;

        private Location _prevLocation;

        private readonly Random _random;

        public override byte? PreferredBrightness => 10;

        public ClockLayout(Identifier identifier): base(identifier)
        {
            _random = new Random();
            _clockElement = new ClockElement(ClockIdentifier);

            _prevLocation = new Location(2, 2);
            AddElement(_prevLocation, _clockElement);
        }

        private void TimerOnElapsed()
        {
            using (LayoutContext.PauseDrawing())
            {
                RemoveElement(_clockElement);

                Location newLocation;
                do
                {
                    newLocation = new Location((byte) _random.Next(LayoutContext.ButtonCount.Width - _clockElement.ButtonCount.Width + 1), (byte) _random.Next(LayoutContext.ButtonCount.Height - _clockElement.ButtonCount.Height + 1));
                } while (newLocation == _prevLocation);

                AddElement(newLocation, _clockElement);
                _prevLocation = newLocation;
            }
        }

        public override void Init()
        {
            base.Init();

            RegisterTimer(new TimeSpan(0,0,0,5), TimerOnElapsed);

            GlobalContext.InitializeEntity(_clockElement);
        }

        public override void ButtonPressed(Location location, ButtonEvent buttonEvent)
        {
            if (buttonEvent == ButtonEvent.Down)
                LayoutContext.SetPreviousLayout();
        }
    }
}
