using System;
using Vkm.Api.Options;

namespace Vkm.Library.Power
{
    [Serializable]
    class PowerOptions: IOptions
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

    public enum PowerAction
    {
        LogOff = 0x00000000,
        Reboot = 0x00000002, 
        PowerOff = 0x00000001
    }
}