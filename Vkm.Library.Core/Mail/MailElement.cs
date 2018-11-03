using System;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;
using Vkm.Library.Common;
using Vkm.Library.Interfaces.Service.Mail;
using Location = Vkm.Api.Basic.Location;


namespace Vkm.Library.Mail
{
    class MailElement: ElementBase
    {
        

        private IMailService[] _mailServices;

        private int? _cachedCount=-1;

        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public MailElement(Identifier identifier) : base(identifier)
        {
        }

        public override void Init()
        {
            base.Init();

            RegisterTimer(new TimeSpan(0,0,0,5), ProcessDraw);

            _mailServices = GlobalContext.GetServices<IMailService>().ToArray();
        }



        int? GetUnreadMessages()
        {
            int? result = null;

            foreach (var service in _mailServices)
            {
                var r = service.GetUnreadMessageCount();
                if (r != null)
                {
                    if (result == null)
                        result = r;
                    else
                        result += r;
                }
            }

            return result;
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            ProcessDraw();
        }

        public override void LeaveLayout()
        {
            base.LeaveLayout();
            _cachedCount = -1;
        }

        private void ProcessDraw()
        {
            var count = GetUnreadMessages();
            if (_cachedCount != count)
            {
                var img = Draw(count, LayoutContext);
                _cachedCount = count;
                DrawInvoke(new[] {new LayoutDrawElement(new Location(0, 0), img)});
            }
        }

        internal static BitmapEx Draw(int? unreadMessageCount, LayoutContext layoutContext)
        {
            var bitmap = layoutContext.CreateBitmap();

            string countStr;
            string example = "8888";
            string symb;
            if (unreadMessageCount == 0)
            {
                symb = FontAwesomeRes.fa_envelope_o;
                countStr = "";
            }
            else if (unreadMessageCount == null)
            {
                symb = FontAwesomeRes.fa_envelope_o;
                countStr = "X";
            }
            else
            {
                symb = FontAwesomeRes.fa_envelope;
                countStr = unreadMessageCount.ToString();
            }

            DefaultDrawingAlgs.DrawIconAndText(bitmap, FontService.Instance.AwesomeFontFamily, symb, layoutContext.Options.Theme.FontFamily, countStr, example, layoutContext.Options.Theme.ForegroundColor);

            return bitmap;
        }

        public override bool ButtonPressed(Location location, ButtonEvent buttonEvent)
        {
            if (buttonEvent == ButtonEvent.Down)
            {
                foreach (var service in _mailServices)
                {
                    service.Activate();
                }
                
                return true;
            }
            return base.ButtonPressed(location, buttonEvent);
        }
    }
}
