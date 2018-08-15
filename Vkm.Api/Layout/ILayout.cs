using System;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Identification;

namespace Vkm.Api.Layout
{
    public interface ILayout: IIdentifiable
    {
        byte? PreferredBrightness { get; }

        event EventHandler<DrawEventArgs> DrawLayout;

        void EnterLayout(LayoutContext layoutContext);
        void LeaveLayout();

        void ButtonPressed(Location location, bool isDown);
    }
}
