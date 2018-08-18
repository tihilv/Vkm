using System.Drawing;
using System.Windows.Forms;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Library.Common;

namespace Vkm.Library.Buttons
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

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            DrawInvoke(new [] {new LayoutDrawElement(new Location(0, 0), DrawKey())});
        }

        private Bitmap DrawKey()
        {
            var bitmap = LayoutContext.CreateBitmap();

            var fontFamily = GlobalContext.Options.Theme.FontFamily;

            DefaultDrawingAlgs.DrawText(bitmap, fontFamily, _text, "W", GlobalContext.Options.Theme.ForegroundColor);

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
