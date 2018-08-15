using System;
using Vkm.Api.Identification;
using Vkm.Api.Transition;
using Vkm.Common;

namespace Vkm.Library.IdleTransition
{
    [Serializable]
    public class IdleTransitionOptions : TransitionOptions
    {
        private TimeSpan _idleTimeout;

        public TimeSpan IdleTimeout
        {
            get => _idleTimeout;
            set => _idleTimeout = value;
        }

        private IdleTransitionOptions()
        {
            LayoutId = Identifiers.DefaultScreenSaverLayout;
            IdleTimeout = new TimeSpan(0, 0, 5, 0);
        }

        public IdleTransitionOptions(Identifier deviceId): this()
        {
            DeviceId = deviceId;
        }
    }
}