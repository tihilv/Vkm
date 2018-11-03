using System;
using System.Collections.Generic;
using Vkm.Api.Identification;

namespace Vkm.Api.Options
{
    [Serializable]
    public class GlobalOptions: IOptions
    {
        public static readonly Identifier Identifier = new Identifier("CoreContext.Options");

        private byte _brightness;

        private readonly List<Identifier> _diabledServices;

        private readonly LayoutLoadOptions _layoutLoadOptions;

        private readonly ThemeOptions _theme;

        private readonly TransitionLoadOptions _transitionLoadOptions;

        private TimeSpan _longPressTimeout;

        public LayoutLoadOptions LayoutLoadOptions => _layoutLoadOptions;

        public TransitionLoadOptions TransitionLoadOptions => _transitionLoadOptions;

        public List<Identifier> DiabledServices => _diabledServices;

        public ThemeOptions Theme => _theme;

        public GlobalOptions()
        {
            _layoutLoadOptions = new LayoutLoadOptions();
            _transitionLoadOptions = new TransitionLoadOptions();
            _theme = new ThemeOptions();
            _diabledServices = new List<Identifier>();

            _brightness = 50;
            _longPressTimeout = TimeSpan.FromMilliseconds(500);
        }

        public byte Brightness
        {
            get => _brightness;
            set => _brightness = value;
        }

        public TimeSpan LongPressTimeout
        {
            get => _longPressTimeout;
            set => _longPressTimeout = value;
        }
    }
}
