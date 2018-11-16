using System;
using System.Collections.Generic;
using Vkm.Intercom.Channels;

namespace Vkm.Intercom.Dispatchers
{
    public class IntercomMasterDispatcher<TService, TCallback>: IDispatcherService, IDisposable where TService: IRemoteService where TCallback: IRemoteService
    {
        private readonly string _pipeName;
        private readonly Func<TService> _serviceFactory;
        
        private readonly List<IntercomDuplexChannel<TService, TCallback>> _dispatchedChannels;
        
        private readonly IntercomServerChannel<IDispatcherService> _serverChannel;

        public IntercomMasterDispatcher(string pipeName, Func<TService> serviceFactory)
        {
            _pipeName = pipeName;
            _serviceFactory = serviceFactory;
            _dispatchedChannels = new List<IntercomDuplexChannel<TService, TCallback>>();
            
            _serverChannel = new IntercomServerChannel<IDispatcherService>(this, pipeName + Constants.Dispatcher, true);
        }

        public string Dispatch()
        {
            var pipeName = $"{_pipeName}_{Guid.NewGuid()}";

            var service = _serviceFactory();
            var newDuplexChannel = new IntercomDuplexChannel<TService, TCallback>(service, pipeName, true);
            
            if (service is IRemoteServiceWithCallback withCallback)
                withCallback.RegisterChannel(newDuplexChannel);

            if (service is IDisposable disposableService)
                newDuplexChannel.ConnectionClosed += (sender, args) => disposableService.Dispose();

            _dispatchedChannels.Add(newDuplexChannel);

            newDuplexChannel.ConnectionClosed += OnConnectionClosed;
            
            return pipeName;
        }

        private void OnConnectionClosed(object sender, EventArgs e)
        {
            var channel = (IntercomDuplexChannel<TService, TCallback>) sender;
            channel.ConnectionClosed -= OnConnectionClosed;
            _dispatchedChannels.Remove(channel);
        }

        public void Dispose()
        {
            foreach (var duplexChannel in _dispatchedChannels)
                duplexChannel.Dispose();
            
            _serverChannel?.Dispose();
        }
    }
}