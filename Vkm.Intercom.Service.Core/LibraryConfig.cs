using Vkm.Api.Configurator;
using Vkm.Api.Device;
using Vkm.Api.Identification;
using Vkm.Api.Module;
using Vkm.Api.Options;
using Vkm.Intercom.Service.Visual;

namespace Vkm.Intercom.Service
{
    class LibraryConfig : IConfigurator
    {
        public Identifier Id => new Identifier("Vkm.Intercom.Configurator");
        public string Name => "Default Library Configurator";
        public IDevice[] Devices { get; set; }
        public GlobalOptions GlobalOptions { get; set; }


        public void Configure(IOptionsService optionsService)
        {
            GlobalOptions.TransitionLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(IntercomTransitionFactory.Identifier, IntercomTransition.Identifier));
        }
    }
}