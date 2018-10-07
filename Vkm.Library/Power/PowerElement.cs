using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Common;
using Vkm.Common.Win32.Win32;
using Vkm.Library.Common;

namespace Vkm.Library.Power
{
    class PowerElement: ElementBase, IOptionsProvider
    {
        private PowerOptions _options;
        
        private readonly PowerAction? _overridenAction;

        private System.Timers.Timer _timer;

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

            if (_options.PressToActionTimeout.TotalMilliseconds > 0)
            {
                _timer = new System.Timers.Timer(_options.PressToActionTimeout.TotalMilliseconds);
                _timer.Elapsed += TimerElapsed;
                _timer.AutoReset = false;
            }
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

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

        private bool _isDown;
        public override bool ButtonPressed(Location location, bool isDown)
        {
            if (isDown)
            {
                _isDown = true;
                if (_timer != null)
                    _timer.Start();
                else
                    ExecuteAction();
            }
            else
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    if (_isDown)
                        LayoutContext.SetLayout(GlobalContext.InitializeEntity(new PowerLayout(Id)));
                }
                _isDown = false;
            }

            return base.ButtonPressed(location, isDown);
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ExecuteAction();
        }

        private void ExecuteAction()
        {
            Win32.DoExitWin((int)GetAction());
        }
    }
}
