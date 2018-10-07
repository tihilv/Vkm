using Vkm.Api.Identification;
using Vkm.Api.Transition;

namespace Vkm.Library.StartupTransition
{
    public class SimpleStartupTransitionFactory : ITransitionFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.DefaultStartupTransition.Factory");

        public Identifier Id => Identifier;

        public string Name => "Simple Startup Transitions";

        public ITransition CreateTransition(Identifier id)
        {
            return new SimpleStartupTransition(id);
        }
    }
}