using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Library.Interfaces.Service.Remote;

namespace Vkm.Library.Service.Harmony
{
    public class HarmonyControlService: IRemoteControlService, IInitializable
    {
        private const string PowerToggle = "PowerToggle";
        private const string VolumeUp = "VolumeUp";
        private const string VolumeDown = "VolumeDown";

        public static Identifier Identifier = new Identifier("Vkm.HarmonyRemoteService");
        public Identifier Id => Identifier;
        public string Name => "Harmony Remote Service";

        private HarmonyHub.Client _harmonyHub;
        private HarmonyHub.Config _config;
        private string _currentActivityId;

        public event EventHandler<ActionEventArgs> ActiveActionChanged;

        public void InitContext(GlobalContext context)
        {
            
        }

        public void Init()
        {
            DiscoveryService service = new DiscoveryService();
            service.HubFound += async (sender, e) => {
                // stop discovery once we've found one hub
                //service.StopDiscovery();
                try
                {
                    _harmonyHub = new HarmonyHub.Client(e.HubInfo.IP);
                    _harmonyHub.OnActivityChanged += CurrentActivityUpdated;
                    await _harmonyHub.OpenAsync();

                    _config = await _harmonyHub.GetConfigAsync();

                    _currentActivityId = await _harmonyHub.GetCurrentActivityAsync();
                }
                catch
                {
                    // nothing to fix
                }
            };
            service.StartDiscovery();
        }

        private void CurrentActivityUpdated(object sender, string e)
        {
            _currentActivityId = e;
            ActiveActionChanged?.Invoke(this, new ActionEventArgs(e));
        }

        public async Task<List<ActionInfo>> GetActions()
        {
            while (_config == null)
                await Task.Delay(500);

            List<ActionInfo> result = new List<ActionInfo>();

            foreach (var activity in _config.Activities)
            {
                result.Add(new ActionInfo(activity.Id, activity.Label, activity.Id == _currentActivityId));
            }

            foreach (var device in _config.Devices)
            {
                result.Add(new ActionInfo(device.Id, PowerToggle, device.Label));
            }
            
            result.Add(new ActionInfo(null, VolumeUp, "^"));
            result.Add(new ActionInfo(null, VolumeDown, "v"));

            return result;
        }

        public void StartAction(ActionInfo action)
        {
            if (action.Command != null)
                _harmonyHub.SendCommandAsync(action.Device, action.Command);
            else
                _harmonyHub.StartActivityAsync(action.Id);
        }
    }
}