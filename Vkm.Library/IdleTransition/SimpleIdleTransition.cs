using System;
using System.Linq;
using Vkm.Api;
using Vkm.Api.Identification;
using Vkm.Api.Options;
using Vkm.Api.Transition;

namespace Vkm.Library.IdleTransition
{
    internal class SimpleIdleTransition: TransitionBase<IdleTransitionOptions>
    {
        private bool _transfered;

        public SimpleIdleTransition(Identifier id) : base(id)
        {
        }

        public override IOptions GetDefaultOptions()
        {
            return new IdleTransitionOptions(GlobalContext.Devices.FirstOrDefault()?.Id??new Identifier());
        }

        public override void Init()
        {
            RegisterTimer(new TimeSpan(0,0,0,5), TimerOnElapsed);
            WorkstationLockService.Instance.LockChanged += InstanceOnLockChanged;
        }

        private void InstanceOnLockChanged(object sender, LockEventArgs e)
        {
            if (e.Locked)
                DoIdleTransfer();
        }

        private void TimerOnElapsed()
        {
            Win32.LASTINPUTINFO lastInput = new Win32.LASTINPUTINFO();
            lastInput.cbSize = (uint) System.Runtime.InteropServices.Marshal.SizeOf(lastInput);
            if (Win32.GetLastInputInfo(ref lastInput))
            {
                var idleSpan = TimeSpan.FromMilliseconds((((Environment.TickCount & int.MaxValue) - (lastInput.dwTime & int.MaxValue)) & int.MaxValue));

                if (idleSpan > TransitionOptions.IdleTimeout)
                    DoIdleTransfer();
                else
                {
                    if (_transfered && !WorkstationLockService.Instance.Locked)
                    {
                        _transfered = false;
                        OnTransitionBack();
                    }
                }
            }
        }

        private void DoIdleTransfer()
        {
            if (!_transfered)
            {
                _transfered = true;
                OnTransition();
            }
        }
    }
}
