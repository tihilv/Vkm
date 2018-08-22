using System;
using System.Windows.Forms;
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

            Application.Exit();
        }
    }
}