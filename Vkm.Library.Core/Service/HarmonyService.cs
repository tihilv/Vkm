using System.Threading.Tasks;
using Vkm.Api.Identification;
using Vkm.Api.Service;

namespace Vkm.Library.Service
{
    public class HarmonyService: ISelfHostedService
    {
        private static readonly Identifier Identifier = new Identifier("Vkm.HarmonyService");
        
        public Identifier Id => Identifier;

        public string Name => "Harmony Hub Service";

        public HarmonyService()
        {
            //DiscoveryService Service = new DiscoveryService();
            //Service.HubFound += async (sender, e) => {
            //    // stop discovery once we've found one hub
            //    //Service.StopDiscovery();

            //    Hub Harmony = new Hub(e.HubInfo);
            //    await Harmony.ConnectAsync(DeviceID.GetDeviceDefault());
            //    await Harmony.SyncConfigurationAsync();

            //    // start activity watch tv
            //    Activity WatchTV = Harmony.GetActivityByLabel("Watch TV");
            //    await Harmony.StartActivity(WatchTV);

            //    // tune to ESPN
            //    await Harmony.ChangeChannel("206");
            //    PlaybackWrapper PlaybackControls = new PlaybackWrapper(WatchTV);

            //    // wait for channel to switch, and pause
            //    await Task.Delay(1000);
            //    await Harmony.PressButtonAsync(PlaybackControls.Pause);
            //};
            //Service.StartDiscovery();

        }
        
    }
}