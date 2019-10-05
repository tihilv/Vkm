using System.Diagnostics;
using System.Linq;
using Microsoft.Office.Interop.Outlook;
using Vkm.Api.Identification;
using Vkm.Common.Win32.Win32;
using Vkm.Library.Interfaces.Service.Mail;
using Exception = System.Exception;

namespace Vkm.Library.Service.Mail
{
    class OutlookService: IMailService
    {
        private Application _application;
        private Items _items;

        public Identifier Id => new Identifier("Vkm.OutlookService");
        public string Name => "Outlook Mail Service";

        public void Activate()
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
        }

        Process GetOutlookProc()
        {
            return Process.GetProcessesByName("OUTLOOK").Where(Win32.ProcessAccessibleForCurrentUser).FirstOrDefault();
        }

        public int? GetUnreadMessageCount()
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

        bool IsOutlookRunning()
        {
            return GetOutlookProc() != null;
        }

        void ConnectToOutlook()
        {
            _application = new Application();
            var outlookNameSpace = _application.GetNamespace("MAPI");
            var inbox = outlookNameSpace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);
            _items = inbox.Items.Restrict("[Unread] = true");
        }


    }
}
