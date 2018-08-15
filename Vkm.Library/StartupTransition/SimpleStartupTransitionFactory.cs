using Vkm.Api.Identification;
using Vkm.Api.Transition;
using Vkm.Common;

namespace Vkm.Library.StartupTransition
{
    public class SimpleStartupTransitionFactory : ITransitionFactory
    {
        public Identifier Id => Identifiers.DefaultStartupTransitionFactory;

        public string Name => "Simple Startup Transitions";

        public ITransition CreateTransition(Identifier id)
        {
            return new SimpleStartupTransition(id);
        }
    }
}