using System;
using Vkm.Api.Identification;

namespace Vkm.Api.Transition
{
    public class TransitionEventArgs : EventArgs
    {
        public Identifier DeviceId { get; private set; }
        public Identifier LayoutId { get; private set; }

        public bool Back { get; private set; }

        public TransitionEventArgs(Identifier deviceId, Identifier layoutId)
        {
            DeviceId = deviceId;
            LayoutId = layoutId;
            Back = false;
        }

        public TransitionEventArgs(Identifier deviceId, bool back)
        {
            DeviceId = deviceId;
            Back = back;
        }
    }
}