using System;
using Vkm.Api.Identification;
using Vkm.Api.Transition;

namespace Vkm.Library.ApplicationChangeTransition
{
    [Serializable]
    public class ApplicationChangeTransitionOptions : TransitionOptions
    {
        private string _process;

        public string Process
        {
            get => _process;
            set => _process = value;
        }

        private ApplicationChangeTransitionOptions()
        {
            
        }

        public ApplicationChangeTransitionOptions(Identifier deviceId): this()
        {
            DeviceId = deviceId;
        }
    }
}