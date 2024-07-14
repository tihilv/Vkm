using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Drawable;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Common;
using Vkm.Library.Common;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.Power
{
    class PowerElement: ElementBase, IOptionsProvider
    {
        private IPowerService _powerService;
        
        private PowerOptions _options;
        
        private readonly PowerAction? _overridenAction;

        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public PowerElement(Identifier identifier) : base(identifier)
        {
        }

        public PowerElement(Identifier identifier, PowerAction action) : this(identifier)
        {
            _overridenAction = action;
        }

        public IOptions GetDefaultOptions()
        {
            return new PowerOptions();
        }

        public void InitOptions(IOptions options)
        {
            _options = (PowerOptions) options;
        }

        public override void Init()
        {
            base.Init();
            _powerService = GlobalContext.GetServices<IPowerService>().First();
        }

        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            string symbol = "";
            switch (GetAction())
            {
                case PowerAction.PowerOff:
                    symbol = FontAwesomeRes.fa_power_off;
                    break;
                case PowerAction.LogOff:
                    symbol = FontAwesomeRes.fa_user;
                    break;
                case PowerAction.Reboot:
                    symbol = FontAwesomeRes.fa_undo;
                    break;
            }

            var bitmap = LayoutContext.CreateBitmap();
            DefaultDrawingAlgs.DrawText(bitmap, FontService.Instance.AwesomeFontFamily, symbol, GlobalContext.Options.Theme.ForegroundColor);
            DrawInvoke(new[] {new LayoutDrawElement(new Location(0, 0), bitmap)});
        }

        private PowerAction GetAction()
        {
            return _overridenAction ?? _options.Action;
        }

        public override void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
        {
            if (buttonEvent == ButtonEvent.Down)
            {
                if (!_options.CallLayout)
                    ExecuteAction();
            }
            else if (buttonEvent == ButtonEvent.Up)
            {
                if (_options.CallLayout)
                    LayoutContext.SetLayout(GlobalContext.InitializeEntity(new PowerLayout(Id)));
            }
            else if (buttonEvent == ButtonEvent.LongPress)
                ExecuteAction();
        }

        private void ExecuteAction()
        {
            _powerService.DoPowerAction(GetAction());
            
        }
    }
}
