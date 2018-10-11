using System;
using System.Linq;
using Vkm.Api.Identification;
using Vkm.Api.Options;
using Vkm.Api.Transition;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.IdleTransition
{
    internal class SimpleIdleTransition: TransitionBase<IdleTransitionOptions>
    {
        private bool _transferred;

        private IWorkstationLockService _workstationLockService;

        public SimpleIdleTransition(Identifier id) : base(id)
        {
        }

        public override IOptions GetDefaultOptions()
        {
            return new IdleTransitionOptions(GlobalContext.Devices.FirstOrDefault()?.Id??new Identifier());
        }

        public override void Init()
        {
            _workstationLockService = GlobalContext.GetServices<IWorkstationLockService>().First();
            
            RegisterTimer(new TimeSpan(0,0,0,5), TimerOnElapsed);
        }

        private void TimerOnElapsed()
        {
            var idleSpan = _workstationLockService.GetIdleTimeSpan();
            if (idleSpan!= TimeSpan.Zero)
            {
                if (idleSpan > TransitionOptions.IdleTimeout)
                    DoIdleTransfer();
                else
                {
                    if (_transferred && !_workstationLockService.Locked)
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
