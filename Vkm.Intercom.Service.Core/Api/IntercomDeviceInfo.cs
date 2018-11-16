using System;
using Vkm.Api.Basic;
using Vkm.Api.Identification;

namespace Vkm.Intercom.Service.Api
{
    [Serializable]
    public class IntercomDeviceInfo
    {
        private readonly Identifier _identifier;
        private readonly DeviceSize _buttonCount;
        private readonly IconSize _iconSize;

        public Identifier Identifier => _identifier;

        public DeviceSize ButtonCount => _buttonCount;

        public IconSize IconSize => _iconSize;

        public IntercomDeviceInfo(Identifier identifier, DeviceSize buttonCount, IconSize iconSize)
        {
            _identifier = identifier;
            _buttonCount = buttonCount;
            _iconSize = iconSize;
        }
    }
}