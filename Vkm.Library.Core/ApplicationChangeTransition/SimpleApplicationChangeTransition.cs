using System;
using System.Linq;
using Vkm.Api.Identification;
using Vkm.Api.Options;
using Vkm.Api.Transition;
using Vkm.Library.Interfaces.Services;

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
            var service = GlobalContext.GetServices<ICurrentProcessService>().FirstOrDefault();
            if (service == null)
                throw new ApplicationException("ICurrentProcessService service is not found.");

            service.ProcessEnter += (sender, args) =>
            {
                if (!_entered && args.ProcessName == TransitionOptions.Process)
                {
                    _entered = true;
                    OnTransition();
                }
            };
            service.ProcessExit += (sender, args) =>
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
