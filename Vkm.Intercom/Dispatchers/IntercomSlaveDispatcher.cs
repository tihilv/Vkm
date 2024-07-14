using System.Threading.Tasks;
using Vkm.Intercom.Channels;

namespace Vkm.Intercom.Dispatchers
{
    public static class IntercomSlaveDispatcher<TService, TCallback> where TService: IRemoteService where TCallback: IRemoteService
    {
        public static async Task<IntercomDuplexChannel<TCallback, TService>> CreateSlaveChannelAsync(TCallback callbackService, string pipeName)
        {
            using (var client = new IntercomClientChannel<IDispatcherService>(pipeName + Constants.Dispatcher))
            {
                await client.ConnectAsync();
                
                var name = client.Execute(Constants.DispatchMethod).ToString();
                
                return new IntercomDuplexChannel<TCallback, TService>(callbackService, name, false);
            }
        }
        
        public static IntercomDuplexChannel<TCallback, TService> CreateSlaveChannel(TCallback callbackService, string pipeName)
        {
            using (var client = new IntercomClientChannel<IDispatcherService>(pipeName + Constants.Dispatcher))
            {
                client.Connect();
                
                var name = client.Execute(Constants.DispatchMethod).ToString();
                
                return new IntercomDuplexChannel<TCallback, TService>(callbackService, name, false);
            }
        }

    }
}