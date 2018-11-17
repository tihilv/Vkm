using System;
using System.Threading.Tasks;

namespace Vkm.Intercom.Channels
{
    public class IntercomDuplexChannel<TService, TCallback>: IIntercomClient, IDisposable where TService: IRemoteService where TCallback: IRemoteService 
    {
        private readonly IntercomServerChannel<TService> _intercomServer;
        private readonly IntercomClientChannel<TCallback> _intercomClient;

        private readonly Task _clientInit;

        public event EventHandler<EventArgs> ConnectionClosed;
        
        public IntercomDuplexChannel(TService remoteService, string pipeName, bool master)
        {
            var serviceName = pipeName + (!master ? Constants.Callback : "");
            var clientName = pipeName + (master ? Constants.Callback : "");
            
            _intercomServer = new IntercomServerChannel<TService>(remoteService, serviceName);
            _intercomClient = new IntercomClientChannel<TCallback>(clientName);

            _intercomServer.ConnectionClosed += OnConnectionClosed;
            
            _clientInit = _intercomClient.ConnectAsync();
        }

        public object Execute(string method, params object[] arguments)
        {
            _clientInit.Wait();
            return _intercomClient.Execute(method, arguments);
        }

        private void OnConnectionClosed(object sender, EventArgs e)
        {

            Dispose();
        }

        public void Dispose()
        {
            _intercomServer.ConnectionClosed -= OnConnectionClosed;
            ConnectionClosed?.Invoke(this, EventArgs.Empty);
            _intercomClient?.Dispose();
            _intercomServer?.Dispose();
        }
    }
}