using Vkm.Api.Module;
using Vkm.Api.Options;
using Vkm.Api.Processes;
using Vkm.Api.Time;

namespace Vkm.Api.Data
{
    public class GlobalServices
    {
        public IOptionsService OptionsService { get; private set; }
        public IModulesService ModulesService { get; private set; }
        public ICurrentProcessService CurrentProcessService { get; private set; }
        public ITimerService TimerService { get; private set; }

        public GlobalServices(IOptionsService optionsService, IModulesService modulesService, ICurrentProcessService currentProcessService, ITimerService timerService)
        {
            OptionsService = optionsService;
            ModulesService = modulesService;
            CurrentProcessService = currentProcessService;
            TimerService = timerService;
        }
    }
}