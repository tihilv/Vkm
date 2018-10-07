using Vkm.Api.Identification;
using Vkm.Api.Transition;

namespace Vkm.Library.ApplicationChangeTransition
{
    public class SimpleApplicationChangeTransitionFactory : ITransitionFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.DefaultApplicationChangeTransition.Factory");

        public Identifier Id => Identifier;

        public string Name => "Active Application Transitions";

        public ITransition CreateTransition(Identifier id)
        {
            return new SimpleApplicationChangeTransition(id);
        }
    }
}