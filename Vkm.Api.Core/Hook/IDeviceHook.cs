using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Drawable;
using Vkm.Api.Layout;
using Vkm.Api.Module;

namespace Vkm.Api.Hook
{
    public interface IDeviceHook: IModule
    {
        bool OnKeyEventHook(Location location, ButtonEvent buttonEvent, ILayout layout, LayoutContext layoutContext);
    }
}
