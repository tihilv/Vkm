using System;
using Vkm.Api.Identification;
using Vkm.Api.Options;

namespace Vkm.Api.Transition
{
    [Serializable]
    public abstract class TransitionOptions : IOptions
    {
        private Identifier _deviceId;
        private Identifier _layoutId;

        public Identifier DeviceId
        {
            get => _deviceId;
            set => _deviceId = value;
        }

        public Identifier LayoutId
        {
            get => _layoutId;
            set => _layoutId = value;
        }
    }
}