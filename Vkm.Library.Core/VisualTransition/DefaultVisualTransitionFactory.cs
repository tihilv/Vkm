using Vkm.Api.Identification;
using Vkm.Api.Transition;
using Vkm.Api.VisualEffect;

namespace Vkm.Library.VisualTransition
{
    public class DefaultVisualTransitionFactory: IVisualTransitionFactory
    {
        public static Identifier Identifier = new Identifier("Vmk.DefaultVisualTransition.Factory");

        public Identifier Id => Identifier;

        public string Name => "Default Visual Effects Factory";
        
        public IVisualTransition CreateVisualTransition(TransitionType transitionType)
        {
            if (transitionType == TransitionType.Instant)
                return new InstantTransition();

            if (transitionType == TransitionType.ElementUpdate)
                return new FadeTransition();

            if (transitionType == TransitionType.LayoutChange)
                return new MoveTransition();

            return new InstantTransition();

        }
    }
}