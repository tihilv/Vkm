using System;
using System.Drawing;
using System.Linq;
using Vkm.Api.Basic;

namespace Vkm.Api.Options
{
    [Serializable]
    public class ThemeOptions
    {
        private string _fontFamilyName = FontFamily.GenericSansSerif.Name;
        private Color _foregroundColor = Color.White;
        private Color _backgroundColor = Color.Black;
        private Color _levelColor = Color.DarkGreen;
        private Color _warningColor = Color.DarkRed;
        private BitmapRepresentation _backgroundBitmapRepresentation;

        [NonSerialized]
        private FontFamily _fontFamilyCached;

        public FontFamily FontFamily
        {
            get
            {
                if (_fontFamilyCached == null)
                    _fontFamilyCached = FontFamily.Families.FirstOrDefault(f => f.Name == _fontFamilyName) ?? FontFamily.GenericMonospace;

                return _fontFamilyCached;
            }
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

        public BitmapRepresentation BackgroundBitmapRepresentation
        {
            get => _backgroundBitmapRepresentation;
            set => _backgroundBitmapRepresentation = value;
        }

        public string FontFamilyName
        {
            get => _fontFamilyName;
            set
            {
                if (_fontFamilyName != value)
                {
                    _fontFamilyName = value;
                    _fontFamilyCached = null;
                }
            }
        }
    }
}
