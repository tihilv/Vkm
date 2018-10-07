using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Vkm.Library.Interfaces.Service.Player;

namespace Vkm.Library.Service.Player.Gpmdp.Providers.Websocket
{
    /// <summary>
    /// Connects to GPMDP WebsocketAPI
    /// Documentation: https://github.com/MarshallOfSound/Google-Play-Music-Desktop-Player-UNOFFICIAL-/blob/master/docs/PlaybackAPI_WebSocket.md
    /// </summary>
    public class WebsocketApi : IGpmdpProvider, IDisposable
    {
        #region static

        private static string _connectionUrl = "ws://localhost:5672";

        public static WebsocketApi CreateWebsocketApi()
        {
            Func<ClientWebSocket> func = () =>
            {
                ClientWebSocket client = new ClientWebSocket();

                try
                {
                    client.ConnectAsync(new Uri(WebsocketApi._connectionUrl), CancellationToken.None).Wait();
                }
                catch
                {
                    // nothing to do
                }

                return client;
            };
            return new WebsocketApi(func);
        }

        #endregion

        private readonly Func<ClientWebSocket> _serverConnectionFunc = null;
        private ClientWebSocket _playerConnection;
        private Task _readTask;

        private WebsocketApi(Func<ClientWebSocket> connectionFunc)
        {
            _serverConnectionFunc = connectionFunc;
        }

        public ClientWebSocket ServerConnection
        {
            get
            {
                if (_playerConnection == null || _playerConnection.State != WebSocketState.Open)
                    _playerConnection = _serverConnectionFunc();

                return _playerConnection;
            }
        }

        public bool IsUseable()
        {
            var useable = ServerConnection.State == WebSocketState.Open;

            return useable;
        }

        public Task<PlayingInfo> GetCurrent()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<PlayingEventArgs> PlayingInfoChanged;

        public void Start()
        {
            _readTask = new Task(ReadFromSocket);
            _readTask.Start();
        }

        private void ReadFromSocket()
        {
            while (true)
            {
                var socketMessage = RetrieveMessage().Result;
                JObject message;
                using (var sr = new StreamReader(socketMessage))
                {
                    message = JObject.Parse(sr.ReadToEnd());
                }

                string channel = message["channel"].ToObject<string>();

                this.PerformAction(channel, message["payload"]);
            }
        }

        private void PerformAction(string channel, JToken payload)
        {
            if (channel == Channel.API_VERSION.GetDescription())
                return;

            if (channel == Channel.PLAY_STATE.GetDescription())
            {
                bool state = payload.ToObject<bool>();
                PlayState(state);
                return;
            }

            if (channel == Channel.TRACK.GetDescription())
            {
                Song song = payload.ToObject<Song>();
                NewTrack(song);
                return;
            }
        }

        private async Task<MemoryStream> RetrieveMessage()
        {
            ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[8192]);
            WebSocketReceiveResult result = null;

            var stream = new MemoryStream();
            do
            {
                try
                {
                    result = await ServerConnection.ReceiveAsync(buffer, CancellationToken.None);
                    stream.Write(buffer.Array, buffer.Offset, result.Count);
                }
                catch
                {
                    _playerConnection = null;
                }
            } while (!result.EndOfMessage);

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        private void PlayState(bool isPlaying)
        {
            if (isPlaying)
            {
            }
            else
            {
            }
        }

        private void NewTrack(Song track)
        {
            //PlayingInfoChanged?.Invoke(this, new PlayingEventArgs(track.ToPlayingInfo()));
        }

        public void Dispose()
        {
            _playerConnection?
                .CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None)
                .RunSynchronously();
        }
    }
}