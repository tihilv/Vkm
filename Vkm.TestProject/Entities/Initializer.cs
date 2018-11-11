using System.Linq;
using Vkm.Api.Data;
using Vkm.Api.Device;
using Vkm.Api.Module;
using Vkm.Api.Options;
using Vkm.Api.Time;
using Vkm.Kernel;

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
        private readonly TestDevice _device;
        private readonly LayoutContext _layoutContext;

        public IOptionsService OptionsService => _optionsService;

        public IModulesService ModulesService => _modulesService;

        public ITimerService TimerService => _timerService;

        public GlobalServices GlobalServices => _globalServices;

        public GlobalOptions GlobalOptions => _globalOptions;

        public GlobalContext GlobalContext => _globalContext;

        public LayoutContext LayoutContext => _layoutContext;

        public TestDevice Device => _device;

        public Initializer()
        {
            _globalOptions = new GlobalOptions();
            _optionsService = new TestOptionsService();
            _modulesService = new TestModulesService();
            _timerService = new TimerService();
            _globalServices = new GlobalServices(_optionsService, _modulesService, _timerService); 
            _globalContext = new GlobalContext(_globalServices);
            _device = (TestDevice)_modulesService.GetModules<IDeviceFactory>().First().GetDevices().First();
            _layoutContext = new LayoutContext(_device, _globalContext, (layout) => {}, () => {}, () => { return null;}, () => null);
        }
    }
}