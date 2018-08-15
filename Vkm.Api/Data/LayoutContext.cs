using System;
using System.Drawing;
using System.Drawing.Imaging;
using Vkm.Api.Basic;
using Vkm.Api.Layout;
using Vkm.Api.Options;

namespace Vkm.Api.Data
{
    public class LayoutContext
    {
        private readonly Action<ILayout> _setLayoutAction;
        private readonly Action _setPreviousLayoutAction;
        private readonly GlobalOptions _options;

        public IconSize IconSize { get; private set; }
        public DeviceSize ButtonCount { get; private set; }

        public GlobalOptions Options => _options;

        public LayoutContext(DeviceSize buttonCount, IconSize iconSize, GlobalContext globalContext, Action<ILayout> setLayoutAction, Action setPreviousLayoutAction)
        {
            IconSize = iconSize;
            ButtonCount = buttonCount;
            _options = globalContext.Options;

            _setLayoutAction = setLayoutAction;
            _setPreviousLayoutAction = setPreviousLayoutAction;
        }

        public Bitmap CreateBitmap()
        {
            var result = new Bitmap(IconSize.Width, IconSize.Height, PixelFormat.Format24bppRgb);

            using (var graphics = Graphics.FromImage(result))
            using (var brush = new SolidBrush(_options.Theme.BackgroundColor))
            {
                graphics.FillRectangle(brush, 0, 0, result.Width, result.Height);
            }

            return result;
        }

        public void SetLayout(ILayout layout)
        {
            _setLayoutAction(layout);
        }

        public void SetPreviousLayout()
        {
            _setPreviousLayoutAction();
        }
    }
}
