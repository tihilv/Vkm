using Vkm.Api.Module;
using Vkm.Api.Options;
using Vkm.Api.Time;

namespace Vkm.Api.Data
{
    public class GlobalServices
    {
        public IOptionsService OptionsService { get; private set; }
        public IModulesService ModulesService { get; private set; }
        public ITimerService TimerService { get; private set; }

        public GlobalServices(IOptionsService optionsService, IModulesService modulesService, ITimerService timerService)
        {
            OptionsService = optionsService;
            ModulesService = modulesService;
            TimerService = timerService;
        }
    }
}