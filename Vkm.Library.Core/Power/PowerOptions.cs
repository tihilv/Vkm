using System;
using Vkm.Api.Options;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.Power
{
    [Serializable]
    public class PowerOptions: IOptions
    {
        private PowerAction _action;
        private bool _callLayout;

        public PowerAction Action
        {
            get => _action;
            set => _action = value;
        }

        public bool CallLayout
        {
            get => _callLayout;
            set => _callLayout = value;
        }
    }

    
}