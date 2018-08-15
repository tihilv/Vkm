using System;
using System.Drawing;

namespace Vkm.Core
{
    [Serializable]
    class ThemeOptions
    {
        private FontFamily _fontFamily = FontFamily.GenericSansSerif;
        private Color _foregroundColor = Color.White;
        private Color _backgroundColor = Color.Black;
        private Color _levelColor = Color.DarkGreen;
        private Color _warningColor = Color.DarkRed;

        public FontFamily FontFamily
        {
            get => _fontFamily;
            set => _fontFamily = value;
        }

        public Color ForegroundColor
        {
            get => _foregroundColor;
            set => _foregroundColor = value;
        }

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => _backgroundColor = value;
        }

        public Color LevelColor
        {
            get => _levelColor;
            set => _levelColor = value;
        }

        public Color WarningColor
        {
            get => _warningColor;
            set => _warningColor = value;
        }
    }
}
