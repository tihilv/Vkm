using System.Linq;
using Vkm.Api.Identification;
using Vkm.Api.Options;
using Vkm.Api.Transition;

namespace Vkm.Library.ApplicationChangeTransition
{
    class SimpleApplicationChangeTransition: TransitionBase<ApplicationChangeTransitionOptions>
    {
        public SimpleApplicationChangeTransition(Identifier id) : base(id)
        {
        }
        
        public override IOptions GetDefaultOptions()
        {
            return new ApplicationChangeTransitionOptions(GlobalContext.Devices.FirstOrDefault()?.Id ?? new Identifier());
        }

        private bool _entered;

        public override void Init()
        {
            GlobalContext.Services.CurrentProcessService.ProcessEnter += (sender, args) =>
            {
                if (!_entered && args.ProcessName == TransitionOptions.Process)
                {
                    _entered = true;
                    OnTransition();
                }
            };
            GlobalContext.Services.CurrentProcessService.ProcessExit += (sender, args) =>
            {
                if (_entered && args.ProcessName == TransitionOptions.Process)
                {
                    _entered = false;
                    OnTransitionBack();
                }
            };
        }
    }
}
