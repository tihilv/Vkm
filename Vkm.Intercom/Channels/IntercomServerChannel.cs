using System;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable SYSLIB0011

namespace Vkm.Intercom.Channels
{
    public class IntercomServerChannel<T>: IDisposable where T: IRemoteService
    {
        private readonly string _pipeName;
        private readonly T _service;
        private readonly TypeInfoProvider _typeInfoProvider;
        
        
        private readonly bool _reconnectable;
        
        private NamedPipeClientStream _serviceStream;
        private readonly BinaryFormatter _formatter;

        private readonly CancellationTokenSource _cancellationTokenSource;
        
        private Task _cycleTask;

        public event EventHandler<EventArgs> ConnectionClosed; 
        
        public IntercomServerChannel(T service, string pipeName, bool reconnectable = false)
        {
            _pipeName = pipeName;
            _service = service;
            _reconnectable = reconnectable;
            
            _typeInfoProvider = new TypeInfoProvider(service);
            _formatter = new BinaryFormatter();
            
            _cancellationTokenSource = new CancellationTokenSource();
            
            _cycleTask = ConnectAsync(_cancellationTokenSource.Token);
        }

        NamedPipeClientStream GetStream()
        {
            return new NamedPipeClientStream(".", _pipeName, PipeDirection.InOut, PipeOptions.None);
        }
        
        private async Task ConnectAsync(CancellationToken cancellationToken)
        {
            do
            {
                _serviceStream = GetStream();

                await _serviceStream.ConnectAsync(cancellationToken);

                await Task.Run(() => DoCycle(cancellationToken), cancellationToken);

            } while (!cancellationToken.IsCancellationRequested);
        }

        private void DoCycle(CancellationToken cancellationToken)
        {
            do
            {
                IntercomMessage resultMessage = null;
                try
                {
                    var message = (IntercomMessage) _formatter.Deserialize(_serviceStream);
                    bool oneWay = _typeInfoProvider.GetIsOneWayMethod(message.MethodName);

                    if (oneWay)
                    {
                        Task.Run(() => ExecuteMethod(message));
                        _formatter.Serialize(_serviceStream, new IntercomMessage(Constants.OneWay, null));
                    }
                    else
                    {
                        var result = ExecuteMethod(message);
                    
                        resultMessage = new IntercomMessage(Constants.Result, new object[] {result});
                        _formatter.Serialize(_serviceStream, resultMessage);
                    }
                }
                catch (SerializationException) // connection is broken
                {
                  if (!_reconnectable)
                  {
                      ConnectionClosed?.Invoke(this, EventArgs.Empty);
                      Dispose();
                  }
                }
                catch (Exception ex)
                {
                    resultMessage = new IntercomMessage(Constants.Exception, new object[] {ex});
                    _formatter.Serialize(_serviceStream, resultMessage);
                }

            } while (!cancellationToken.IsCancellationRequested && !_reconnectable);
        }

        private object ExecuteMethod(IntercomMessage message)
        {
            var methodInfo = _typeInfoProvider.GetMethodInfo(message.MethodName);

            return methodInfo.Invoke(_service, message.Arguments);
        }

        
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _serviceStream?.Dispose();
        }
    }
}