using Vkm.Api.Identification;
using Vkm.Api.Transition;
using Vkm.Common;

namespace Vkm.Library.IdleTransition
{
    public class SimpleIdleTransitionFactory : ITransitionFactory
    {
        public Identifier Id => Identifiers.DefaultIdleTransitionFactory;

        public string Name => "Simple System Idle Transitions";

        public ITransition CreateTransition(Identifier id)
        {
            return new SimpleIdleTransition(id);
        }
    }
}