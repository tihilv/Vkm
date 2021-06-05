using System;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Drawable;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;

namespace Vkm.Library.Common
{
    public class BackElement: ElementBase
    {
        public BackElement() : base(new Identifier("Vkm.Back"))
        {
        }

        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            var bitmap = LayoutContext.CreateBitmap();
            DefaultDrawingAlgs.DrawText(bitmap, FontService.Instance.AwesomeFontFamily, FontAwesomeRes.fa_arrow_left, GlobalContext.Options.Theme.ForegroundColor);
            DrawInvoke(new[] {new LayoutDrawElement(new Location(0, 0), bitmap)});

        }

        public override void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
        {
            if (buttonEvent == ButtonEvent.Down)
                LayoutContext.SetPreviousLayout();
        }
    }
}
