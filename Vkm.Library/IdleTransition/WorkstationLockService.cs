using System;
using Microsoft.Win32;

namespace Vkm.Library.IdleTransition
{
    class WorkstationLockService
    {
        internal static readonly WorkstationLockService Instance = new WorkstationLockService();

        public bool Locked { get; private set; }

        public event EventHandler<LockEventArgs> LockChanged;

        private WorkstationLockService()
        {
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
        }

        void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                Locked = true;
                LockChanged?.Invoke(this, new LockEventArgs(Locked));
            }

            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                Locked = false;
                LockChanged?.Invoke(this, new LockEventArgs(Locked));
            }
        }
    }
}