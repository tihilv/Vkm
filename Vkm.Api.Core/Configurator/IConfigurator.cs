using Vkm.Api.Device;
using Vkm.Api.Module;
using Vkm.Api.Options;

namespace Vkm.Api.Configurator
{
    public interface IConfigurator: IModule
    {
        IDevice[] Devices { get; set; }
        GlobalOptions GlobalOptions { get; set; }
        void Configure(IOptionsService optionsService);
    }
}
