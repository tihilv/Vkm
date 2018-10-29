using System;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Common;

namespace Vkm.Library.Buttons
{
    public class MoveToLayoutElement: ElementBase, IOptionsProvider
    {
        private MoveToElementOptions _options;

        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public MoveToLayoutElement(Identifier identifier) : base(identifier)
        {
        }
        
        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            DrawKey();
        }
        
        private void DrawKey()
        {
            var bitmap = LayoutContext.CreateBitmap();

            var fontFamily = GlobalContext.Options.Theme.FontFamily;

            DefaultDrawingAlgs.DrawText(bitmap, fontFamily, _options.Text, GlobalContext.Options.Theme.ForegroundColor);

            DrawInvoke(new [] {new LayoutDrawElement(new Location(0, 0), bitmap)});
        }

        public override bool ButtonPressed(Location location, bool isDown)
        {
            if (isDown && location.X == 0 && location.Y == 0)
            {
                LayoutContext.SetLayout(_options.LayoutIdentifier);

                return true;
            }

            return false;
        }

        public IOptions GetDefaultOptions()
        {
            return new MoveToElementOptions();
        }

        public void InitOptions(IOptions options)
        {
            _options = (MoveToElementOptions) options;
        }
    }

    [Serializable]
    public class MoveToElementOptions : IOptions
    {
        private Identifier _layoutIdentifier;
        private string _text;

        public Identifier LayoutIdentifier
        {
            get => _layoutIdentifier;
            set => _layoutIdentifier = value;
        }

        public string Text
        {
            get => _text;
            set => _text = value;
        }
    }
}
