using Vkm.Api.Identification;
using Vkm.Api.Transition;

namespace Vkm.Intercom.Service.Visual
{
    public class IntercomTransitionFactory : ITransitionFactory
    {
        internal static Identifier Identifier => new Identifier("Vkm.Intercom.Transition.Factory");

        internal static readonly IntercomTransition Transition = new IntercomTransition(IntercomTransition.Identifier);

        public Identifier Id => Identifier;

        public string Name => "Intercom Transition Factory";

        public ITransition CreateTransition(Identifier id)
        {
            return Transition;
        }
    }
}