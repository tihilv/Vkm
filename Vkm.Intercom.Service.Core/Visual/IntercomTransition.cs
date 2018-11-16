using System;
using Vkm.Api.Identification;
using Vkm.Api.Transition;

namespace Vkm.Intercom.Service.Visual
{
    public class IntercomTransition : ITransition
    {
        internal static Identifier Identifier => new Identifier("Vkm.Intercom.Transition");

        private readonly Identifier _identifier;

        public Identifier Id => _identifier;

        public event EventHandler<TransitionEventArgs> PerformTransition;

        public IntercomTransition(Identifier identifier)
        {
            _identifier = identifier;
        }

        public void Run()
        {

        }

        public void SwitchToLayout(Identifier deviceId, Identifier layoutId)
        {
            PerformTransition?.Invoke(this, new TransitionEventArgs(deviceId, layoutId));
        }
    }
}