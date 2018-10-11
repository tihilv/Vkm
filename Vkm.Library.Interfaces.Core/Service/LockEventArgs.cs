using System;

namespace Vkm.Library.Interfaces.Service
{
    public class LockEventArgs : EventArgs
    {
        public bool Locked { get; private set; }

        public LockEventArgs(bool locked)
        {
            Locked = locked;
        }
    }
}