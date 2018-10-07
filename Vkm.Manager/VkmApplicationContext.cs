using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Vkm.Api.Basic;
using Vkm.Api.Win32;
using Vkm.Core;
using Vkm.Manager.Properties;

namespace Vkm.Manager
{
    //static class Logger
    //{
    //    private static string _fileName = "log.log";

    //    public static void Write(string msg)
    //    {
    //        using (StreamWriter sw = new StreamWriter(_fileName, true, Encoding.UTF8))
    //            sw.WriteLine($"[{DateTime.Now}] {msg}");
    //    }
    //}

    public class VkmApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;

        private VkmCore _coreContext;

        public VkmApplicationContext ()
        {
            _trayIcon = InitTrayIcon();

            using (WindowsSession session = new WindowsSession())
            {
                while (session.IsLocked())
                {
                    Thread.Sleep(5000);
                }
            }

            _coreContext = new VkmCore();
            
            SystemEvents.SessionSwitch += SystemEventsOnSessionSwitch;
            SystemEvents.SessionEnding += SystemEventsOnSessionEnding;
        }

        private void SystemEventsOnSessionEnding(object sender, SessionEndingEventArgs e)
        {
            DoExit();
        }

        private void SystemEventsOnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLogon || e.Reason == SessionSwitchReason.SessionUnlock)
            {
                _coreContext.Resume();
            }
            else if (e.Reason == SessionSwitchReason.SessionLogoff || e.Reason == SessionSwitchReason.SessionLock)
            {
                _coreContext.Pause();
            }
        }

        private NotifyIcon InitTrayIcon()
        {
            return new NotifyIcon()
            {
                Icon = Resources.TrayIcon,
                ContextMenu = new ContextMenu(new[] {new MenuItem("Exit", Exit)}),
                Visible = true
            };
        }

        void Exit(object sender, EventArgs e)
        {
            DoExit();
        }

        void DoExit()
        {
            _trayIcon.Visible = false;

            DisposeHelper.DisposeAndNull(ref _coreContext);

            SystemEvents.SessionSwitch -= SystemEventsOnSessionSwitch;

            Application.Exit();
        }
    }
}