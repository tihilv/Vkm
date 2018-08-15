using System;
using System.Linq;
using Vkm.Api.Identification;
using Vkm.Api.Options;
using Vkm.Api.Transition;
using Vkm.Common;

namespace Vkm.Library.ApplicationChangeTransition
{
    class SimpleApplicationChangeTransition: TransitionBase<ApplicationChangeTransitionOptions>
    {
        public SimpleApplicationChangeTransition(Identifier id) : base(id)
        {
        }
        
        public override IOptions GetDefaultOptions()
        {
            var result = new ApplicationChangeTransitionOptions(GlobalContext.Devices.FirstOrDefault()?.Id ?? new Identifier());
            if (Id == Identifiers.DefaultApplicationChangeTransitionCalc)
            {
                result.Process = "Calculator";
                result.LayoutId = Identifiers.DefaultNumpadLayout;
            }
            if (Id == Identifiers.DefaultApplicationChangeTransitionExcel)
            {
                result.Process = "EXCEL";
                result.LayoutId = Identifiers.DefaultNumpadLayout;
            }
            if (Id == Identifiers.DefaultApplicationChangeTransitionTotalCmd)
            {
                result.Process = "TOTALCMD64";
                result.LayoutId = Identifiers.DefaultNumpadLayout;
            }


            return result;
        }

        private bool _entered;

        public override void Init()
        {
            GlobalContext.Services.CurrentProcessService.ProcessEnter += (sender, args) =>
            {
                if (!_entered && args.ProcessName == TransitionOptions.Process)
                {
                    _entered = true;
                    OnTransition();
                }
            };
            GlobalContext.Services.CurrentProcessService.ProcessExit += (sender, args) =>
            {
                if (_entered && args.ProcessName == TransitionOptions.Process)
                {
                    _entered = false;
                    OnTransitionBack();
                }
            };
        }
    }
}
