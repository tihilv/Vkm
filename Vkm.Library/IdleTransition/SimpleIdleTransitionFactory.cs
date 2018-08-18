using Vkm.Api.Identification;
using Vkm.Api.Transition;

namespace Vkm.Library.IdleTransition
{
    public class SimpleIdleTransitionFactory : ITransitionFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.DefaultIdleTransition.Factory");

        public Identifier Id => Identifier;

        public string Name => "Simple System Idle Transitions";

        public ITransition CreateTransition(Identifier id)
        {
            return new SimpleIdleTransition(id);
        }
    }
}