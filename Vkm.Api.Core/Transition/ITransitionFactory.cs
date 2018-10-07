using Vkm.Api.Identification;
using Vkm.Api.Module;

namespace Vkm.Api.Transition
{
    public interface ITransitionFactory : IModule
    {
        ITransition CreateTransition(Identifier id);
    }
}