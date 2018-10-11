using System;
using Vkm.Api.Options;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.Power
{
    [Serializable]
    public class PowerOptions: IOptions
    {
        private TimeSpan _pressToActionTimeout;
        private PowerAction _action;

        public TimeSpan PressToActionTimeout
        {
            get => _pressToActionTimeout;
            set => _pressToActionTimeout = value;
        }

        public PowerAction Action
        {
            get => _action;
            set => _action = value;
        }
    }

    
}