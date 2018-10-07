using System.Linq;
using Vkm.Api.Identification;
using Vkm.Api.Options;
using Vkm.Api.Transition;

namespace Vkm.Library.StartupTransition
{
    internal class SimpleStartupTransition: TransitionBase<StartupTransitionOptions>
    {
        public SimpleStartupTransition(Identifier id): base(id)
        {

        }

        public override IOptions GetDefaultOptions()
        {
            return new StartupTransitionOptions(GlobalContext.Devices.FirstOrDefault()?.Id ?? new Identifier());
        }

        
        public override void Init()
        {
            OnTransition();
        }
    }
}
