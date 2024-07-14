using System;
using Vkm.Api.Basic;

namespace Vkm.Api.Device
{
    public class ButtonEventArgs : EventArgs
    {
        public Location Location { get; private set; }
        public bool IsDown { get; private set; }

        public ButtonEventArgs(Location location, bool isDown)
        {
            Location = location;
            IsDown = isDown;
        }
    }
}