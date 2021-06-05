using System;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Api.Element
{
    public interface IElement: IIdentifiable
    {
        DeviceSize ButtonCount { get; }

        event EventHandler<DrawEventArgs> DrawElement;

        void EnterLayout(LayoutContext layoutContext, ILayout previousLayout);
        void LeaveLayout();

        void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext);
    }
}