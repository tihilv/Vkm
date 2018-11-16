using System.Threading.Tasks;
using Vkm.Api.Basic;
using Vkm.Api.Identification;
using Vkm.Intercom.Channels;
using Vkm.Intercom.Dispatchers;
using Vkm.Intercom.Service.Api;

namespace Vkm.Intercom.Service
{
    public class VkmIntercomClient : IVkmRemoteService
    {
        private readonly IntercomDuplexChannel<IVkmRemoteCallback, IVkmRemoteService> _channel;

        private VkmIntercomClient(IntercomDuplexChannel<IVkmRemoteCallback, IVkmRemoteService> channel)
        {
            _channel = channel;
        }

        public static async Task<VkmIntercomClient> CreateAsync(IVkmRemoteCallback callback)
        {
            return new VkmIntercomClient(await IntercomSlaveDispatcher<IVkmRemoteService, IVkmRemoteCallback>.CreateSlaveChannelAsync(callback, Constants.DispatcherName));
        }

        public static VkmIntercomClient Create(IVkmRemoteCallback callback)
        {
            return new VkmIntercomClient(IntercomSlaveDispatcher<IVkmRemoteService, IVkmRemoteCallback>.CreateSlaveChannel(callback, Constants.DispatcherName));
        }
        
        public IntercomDeviceInfo[] GetDevices()
        {
            return (IntercomDeviceInfo[]) _channel.Execute(nameof(GetDevices));
        }

        public Identifier GetLayout(string name)
        {
            return (Identifier) _channel.Execute(nameof(GetLayout), name);
        }

        public void RemoveLayout(Identifier layoutId)
        {
            _channel.Execute(nameof(RemoveLayout), layoutId);
        }

        public void SwitchToLayout(Identifier deviceId, Identifier layoutId)
        {
            _channel.Execute(nameof(SwitchToLayout), deviceId, layoutId);
        }

        public void SetBitmap(Identifier layoutId, Location location, byte[] bitmapBytes)
        {
            _channel.Execute(nameof(SetBitmap), layoutId, location, bitmapBytes);
        }

        public void RemoveBitmap(Identifier layoutId, Location location)
        {
            _channel.Execute(nameof(RemoveBitmap), layoutId, location);
        }
    }
}