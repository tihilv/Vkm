using Vkm.Api.Basic;
using Vkm.Api.Layout;
using Vkm.Api.Module;

namespace Vkm.Api.Hook
{
    public interface IDeviceHook: IModule
    {
        bool OnKeyEventHook(Location location, ButtonEvent buttonEvent, ILayout layout);
    }
}
