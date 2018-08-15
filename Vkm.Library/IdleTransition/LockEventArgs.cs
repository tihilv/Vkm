using System;

namespace Vkm.Library.IdleTransition
{
    class LockEventArgs : EventArgs
    {
        public bool Locked { get; private set; }

        public LockEventArgs(bool locked)
        {
            Locked = locked;
        }
    }
}