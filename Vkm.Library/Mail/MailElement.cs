using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Outlook;
using Vkm.Api;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Library.Common;
using Exception = System.Exception;
using Location = Vkm.Api.Basic.Location;


namespace Vkm.Library.Mail
{
    class MailElement: ElementBase
    {
        private Application _application;
        private Items _items;


        private int? _cachedCount=-1;

        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public MailElement(Identifier identifier) : base(identifier)
        {
        }

        public override void Init()
        {
            base.Init();

            RegisterTimer(new TimeSpan(0,0,0,5), ProcessDraw);
        }

        void ConnectToOutlook()
        {
            _application = new Application();
            var outlookNameSpace = _application.GetNamespace("MAPI");
            var inbox = outlookNameSpace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);
            _items = inbox.Items.Restrict("[Unread] = true");
        }

        Process GetOutlookProc()
        {
            return Process.GetProcessesByName("OUTLOOK").FirstOrDefault();
        }

        bool IsOutlookRunning()
        {
            return GetOutlookProc() != null;
        }

        int? GetUnreadMessages()
        {
            try
            {
                if (_application == null && IsOutlookRunning())
                    ConnectToOutlook();

                return _items?.Count;
            }
            catch (Exception)
            {
                _application = null;
                _items = null;
                return null;
            }
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

        internal static Bitmap Draw(int? unreadMessageCount, LayoutContext layoutContext)
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

        public override bool ButtonPressed(Location location, bool isDown)
        {
            if (isDown)
            {
                var proc = GetOutlookProc();
                if (proc == null)
                {
                    Process.Start("outlook.exe");
                }
                else
                {
                    Win32.SwitchToThisWindow(proc.MainWindowHandle, true);
                }
                return true;
            }
            return base.ButtonPressed(location, isDown);
        }
    }
}
