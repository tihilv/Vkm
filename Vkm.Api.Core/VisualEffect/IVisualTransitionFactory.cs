using Vkm.Api.Module;
using Vkm.Api.VisualEffect;

namespace Vkm.Api.Transition
{
    public interface IVisualTransitionFactory : IModule
    {
        IVisualTransition CreateVisualTransition(TransitionType transitionType);
    }
}