using System;

namespace Vkm.Api.Options
{
    [Serializable]
    public class GlobalOptions: IOptions
    {
        private byte _brightness;
        private readonly ThemeOptions _theme;

        public GlobalOptions()
        {
            _brightness = 50;
            _theme = new ThemeOptions();
        }

        public byte Brightness
        {
            get => _brightness;
            set => _brightness = value;
        }

        public ThemeOptions Theme => _theme;
    }
}
