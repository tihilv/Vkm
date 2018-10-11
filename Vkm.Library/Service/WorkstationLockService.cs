using System;
using Microsoft.Win32;
using Vkm.Api.Identification;
using Vkm.Common.Win32.Win32;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.Service
{
    public class WorkstationLockService : IWorkstationLockService
    {
        public Identifier Id => new Identifier("Vkm.WorkstationLockService");
        public string Name => "Workstation Lock Service";
        
        public bool Locked { get; private set; }

        public event EventHandler<LockEventArgs> LockChanged;

        public WorkstationLockService()
        {
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
        }

        public TimeSpan GetIdleTimeSpan()
        {
            Win32.LASTINPUTINFO lastInput = new Win32.LASTINPUTINFO();
            lastInput.cbSize = (uint) System.Runtime.InteropServices.Marshal.SizeOf(lastInput);
            if (Win32.GetLastInputInfo(ref lastInput))
            {
                var idleSpan = TimeSpan.FromMilliseconds((((Environment.TickCount & int.MaxValue) - (lastInput.dwTime & int.MaxValue)) & int.MaxValue));
                return idleSpan;
            }

            return TimeSpan.Zero;
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