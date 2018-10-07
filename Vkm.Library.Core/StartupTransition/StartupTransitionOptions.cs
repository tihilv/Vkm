using System;
using Vkm.Api.Identification;
using Vkm.Api.Transition;

namespace Vkm.Library.StartupTransition
{
    [Serializable]
    public class StartupTransitionOptions : TransitionOptions
    {
        public StartupTransitionOptions(Identifier deviceId)
        {
            DeviceId = deviceId;
        }
    }
}