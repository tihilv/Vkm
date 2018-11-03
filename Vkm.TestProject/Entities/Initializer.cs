using System.Linq;
using Vkm.Api.Data;
using Vkm.Api.Device;
using Vkm.Api.Module;
using Vkm.Api.Options;
using Vkm.Api.Time;

namespace Vkm.TestProject.Entities
{
    internal class Initializer
    {
        private readonly IOptionsService _optionsService;
        private readonly IModulesService _modulesService;
        private readonly ITimerService _timerService;
        private readonly GlobalServices _globalServices;
        private readonly GlobalOptions _globalOptions;
        private readonly GlobalContext _globalContext;
        private readonly IDevice _device;
        private readonly LayoutContext _layoutContext;

        public IOptionsService OptionsService => _optionsService;

        public IModulesService ModulesService => _modulesService;

        public ITimerService TimerService => _timerService;

        public GlobalServices GlobalServices => _globalServices;

        public GlobalOptions GlobalOptions => _globalOptions;

        public GlobalContext GlobalContext => _globalContext;

        public LayoutContext LayoutContext => _layoutContext;

        public IDevice Device => _device;

        public Initializer()
        {
            _globalOptions = new GlobalOptions();
            _optionsService = new TestOptionsService();
            _modulesService = new TestModulesService();
            _timerService = new TestTimerService();
            _globalServices = new GlobalServices(_optionsService, _modulesService, _timerService); 
            _globalContext = new GlobalContext(_globalOptions, _globalServices, null, null, null, null);
            _device = _modulesService.GetModules<IDeviceFactory>().First().GetDevices().First();
            _layoutContext = new LayoutContext(_device, _globalContext, layout => {}, () => {}, () => { return null;});
        }
    }
}