using System.Drawing;
using System.Windows.Forms;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;
using Vkm.Common.Win32.Win32;
using Vkm.Library.Common;

namespace Vkm.Library.Buttons
{
    internal class ButtonElement: ElementBase
    {
        private string _text;
        
        private readonly string _keys;
        private readonly byte? _key;
        
        private readonly FontFamily _fontFamily;
        
        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public ButtonElement(string keys):this(keys, keys){}


        public ButtonElement(string text, string keys): base(new Identifier($"{text}.{keys}"))
        {
            _text = text;
            _keys = keys;
        }

        public ButtonElement(string text, FontFamily fontFamily, byte key): base(new Identifier($"{text}.{key}"))
        {
            _text = text;
            _key = key;

            _fontFamily = fontFamily;
        }
        
        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            DrawKey();
        }
        
        private void DrawKey()
        {
            var bitmap = LayoutContext.CreateBitmap();

            var fontFamily = _fontFamily??GlobalContext.Options.Theme.FontFamily;

            DefaultDrawingAlgs.DrawText(bitmap, fontFamily, _text, GlobalContext.Options.Theme.ForegroundColor);

            DrawInvoke(new [] {new LayoutDrawElement(new Location(0, 0), bitmap)});
        }

        public override bool ButtonPressed(Location location, bool isDown)
        {
            if (isDown && location.X == 0 && location.Y == 0)
            {
                if (_key == null)
                    SendKeys.SendWait(_keys);
                else
                    Win32.ButtonPress(_key.Value);
                
                return true;
            }

            return false;
        }

        public void ReplaceText(string text)
        {
            _text = text;
            DrawKey();
        }
    }
}
