using System;
using System.Windows.Forms;
using Microsoft.Win32;
using Vkm.Api.Basic;
using Vkm.Core;
using Vkm.Manager.Properties;

namespace Vkm.Manager
{
    public class VkmApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;

        private VkmCore _coreContext;

        public VkmApplicationContext ()
        {
            _trayIcon = InitTrayIcon();

            _coreContext = new VkmCore();
            
            SystemEvents.SessionSwitch += SystemEventsOnSessionSwitch;
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
            _trayIcon.Visible = false;

            DisposeHelper.DisposeAndNull(ref _coreContext);

            SystemEvents.SessionSwitch -= SystemEventsOnSessionSwitch;

            Application.Exit();
        }
    }
}