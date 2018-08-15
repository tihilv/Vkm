using Vkm.Api.Identification;
using Vkm.Api.Transition;
using Vkm.Common;

namespace Vkm.Library.ApplicationChangeTransition
{
    public class SimpleApplicationChangeTransitionFactory : ITransitionFactory
    {
        public Identifier Id => Identifiers.DefaultApplicationChangeTransitionFactory;

        public string Name => "Active Application Transitions";

        public ITransition CreateTransition(Identifier id)
        {
            return new SimpleApplicationChangeTransition(id);
        }
    }
}