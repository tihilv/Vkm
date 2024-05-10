using System;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
#pragma warning disable SYSLIB0011

namespace Vkm.Intercom.Channels
{
    public class IntercomClientChannel<T>: IIntercomClient, IDisposable where T: IRemoteService
    {
        private readonly NamedPipeServerStream _serviceStream;
        private readonly BinaryFormatter _formatter;
        private readonly TypeInfoProvider _typeInfoProvider;

        public IntercomClientChannel(string pipeName)
        {
            _serviceStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut);
            _formatter = new BinaryFormatter();
            
            _typeInfoProvider = new TypeInfoProvider(typeof(T));
        }

        public Task ConnectAsync()
        {
            return _serviceStream.WaitForConnectionAsync();
        }

        public void Connect()
        {
            _serviceStream.WaitForConnection();
        }

        object _lock = new object();
        
        public object Execute(string method, params object[] arguments)
        {
            lock (_lock)
            {
                IntercomMessage message = new IntercomMessage(method, arguments);
                _formatter.Serialize(_serviceStream, message);
                var result = (IntercomMessage) _formatter.Deserialize(_serviceStream);

                if (result.MethodName == Constants.Result)
                    return result.Arguments[0];
                else if (result.MethodName == Constants.Exception)
                    throw (Exception) result.Arguments[0];
                else
                    return null;
            }
        }
        
        public void Dispose()
        {
            _serviceStream?.Dispose();
        }
    }
}