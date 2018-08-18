using System;
using Vkm.Api.Identification;

namespace Vkm.Api.Options
{
    [Serializable]
    public class GlobalOptions: IOptions
    {
        public static readonly Identifier Identifier = new Identifier("CoreContext.Options");

        private byte _brightness;
        private readonly ThemeOptions _theme;

        private readonly LayoutLoadOptions _layoutLoadOptions;
        private readonly TransitionLoadOptions _transitionLoadOptions;

        public LayoutLoadOptions LayoutLoadOptions => _layoutLoadOptions;

        public TransitionLoadOptions TransitionLoadOptions => _transitionLoadOptions;

        public GlobalOptions()
        {
            _layoutLoadOptions = new LayoutLoadOptions();
            _transitionLoadOptions = new TransitionLoadOptions();
            _theme = new ThemeOptions();

            _brightness = 50;
        }

        public byte Brightness
        {
            get => _brightness;
            set => _brightness = value;
        }

        public ThemeOptions Theme => _theme;
    }
}
