using System.Drawing;
using System.Windows.Forms;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Library
{
    internal class ButtonElement: ElementBase
    {
        private readonly string _text;
        private readonly string _keys;
        
        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public ButtonElement(string keys):this(keys, keys){}


        public ButtonElement(string text, string keys): base(new Identifier($"{text}.{keys}"))
        {
            _text = text;
            _keys = keys;
        }

        public override void EnterLayout(LayoutContext layoutContext)
        {
            base.EnterLayout(layoutContext);

            DrawElementInvoke(new [] {new LayoutDrawElement(new Location(0, 0), DrawKey())});
        }

        private Bitmap DrawKey()
        {
            var bitmap = LayoutContext.CreateBitmap();

            var fontFamily = GlobalContext.Options.Theme.FontFamily;

            var height = FontEstimation.EstimateFontSize(bitmap, fontFamily, "W");

            using (var graphics = Graphics.FromImage(bitmap))
            using (var whiteBrush = new SolidBrush(GlobalContext.Options.Theme.ForegroundColor))
            using (var font = new Font(fontFamily, height, GraphicsUnit.Pixel))
            {
                var size = graphics.MeasureString(_text, font);
                graphics.DrawString(_text, font, whiteBrush, (bitmap.Width - size.Width)/2, (bitmap.Height - size.Height)/2);
            }

            return bitmap;
        }

        public override bool ButtonPressed(Location location, bool isDown)
        {
            if (isDown && location.X == 0 && location.Y == 0)
            {
                SendKeys.SendWait(_keys);
                return true;
            }

            return false;
        }
    }
}
