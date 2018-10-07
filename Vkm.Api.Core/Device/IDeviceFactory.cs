using Vkm.Api.Module;

namespace Vkm.Api.Device
{
    public interface IDeviceFactory: IModule
    {
        IDevice[] GetDevices();
    }
}