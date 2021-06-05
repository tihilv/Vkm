using System;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Api.Drawable
{
    public interface IDrawable: IIdentifiable
    {
        event EventHandler<DrawEventArgs> DrawRequested;
        void EnterLayout(LayoutContext layoutContext, ILayout previousLayout);
        void LeaveLayout();

        void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext);
    }
}