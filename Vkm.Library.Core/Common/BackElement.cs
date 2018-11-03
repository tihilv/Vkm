using Vkm.Api.Basic;
using Vkm.Api.Data;
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

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            var bitmap = LayoutContext.CreateBitmap();
            DefaultDrawingAlgs.DrawText(bitmap, FontService.Instance.AwesomeFontFamily, FontAwesomeRes.fa_arrow_left, GlobalContext.Options.Theme.ForegroundColor);
            DrawInvoke(new[] {new LayoutDrawElement(new Location(0, 0), bitmap)});

        }

        public override bool ButtonPressed(Location location, ButtonEvent buttonEvent)
        {
            if (buttonEvent == ButtonEvent.Down)
                LayoutContext.SetPreviousLayout();

            return base.ButtonPressed(location, buttonEvent);
        }
    }
}
