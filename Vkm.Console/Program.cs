using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Harmony;
using Harmony.DeviceWrappers;
using Vkm.Intercom;
using WebSocketSharp;

namespace Vkm.Console
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Test();
            Application.Run(new Form1());
        }

        public static async void Test()
        {
            DiscoveryService Service = new DiscoveryService();
            Service.HubFound += async (sender, e) => {
                // stop discovery once we've found one hub
                //Service.StopDiscovery();

                Hub Harmony = new Hub(e.HubInfo);
                await Harmony.ConnectAsync(DeviceID.GetDeviceDefault());
                await Harmony.SyncConfigurationAsync();

                // start activity watch tv
                Activity WatchTV = Harmony.GetActivityByLabel("Watch TV");
                //await Harmony.StartActivity(WatchTV);
                await Harmony.EndActivity();

                // tune to ESPN
                await Harmony.ChangeChannel("206");
                PlaybackWrapper PlaybackControls = new PlaybackWrapper(WatchTV);

                // wait for channel to switch, and pause
                await Task.Delay(1000);
                await Harmony.PressButtonAsync(PlaybackControls.Pause);
            };
            Service.StartDiscovery();
        }
    }
}
