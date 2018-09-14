using System;
using System.Linq;
using Vkm.Api;
using Vkm.Api.Identification;
using Vkm.Api.Options;
using Vkm.Api.Transition;
using Vkm.Api.Win32;

namespace Vkm.Library.IdleTransition
{
    internal class SimpleIdleTransition: TransitionBase<IdleTransitionOptions>
    {
        private bool _transferred;

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
                    if (_transferred && !WorkstationLockService.Instance.Locked)
                    {
                        _transferred = false;
                        OnTransitionBack();
                    }
                }
            }
        }

        private void DoIdleTransfer()
        {
            if (!_transferred)
            {
                _transferred = true;
                OnTransition();
            }
        }
    }
}
