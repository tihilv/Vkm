using System;
using Vkm.Api.Data;
using Vkm.Api.Identification;

namespace Vkm.Api.Transition
{
    public interface ITransition: IIdentifiable
    {
        event EventHandler<TransitionEventArgs> PerformTransition;

        void Run();
    }
}
