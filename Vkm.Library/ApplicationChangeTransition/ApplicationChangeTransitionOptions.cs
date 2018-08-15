using System;
using Vkm.Api.Identification;
using Vkm.Api.Transition;
using Vkm.Common;

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
            LayoutId = Identifiers.DefaultScreenSaverLayout;
        }

        public ApplicationChangeTransitionOptions(Identifier deviceId): this()
        {
            DeviceId = deviceId;
        }
    }
}