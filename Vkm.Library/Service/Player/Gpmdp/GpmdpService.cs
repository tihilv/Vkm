using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Library.Interfaces.Service.Player;
using Vkm.Library.Service.Player.Gpmdp.Providers;
using Vkm.Library.Service.Player.Gpmdp.Providers.Websocket;

namespace Vkm.Library.Service.Player.Gpmdp
{
    public class GpmdpService : IPlayerService, IInitializable, IBitmapDownloader
    {
        private IGpmdpProvider _provider;

        public Identifier Id => new Identifier("Vkm.GpmdpService");
        public string Name => "Google Play Music Service";

        public event EventHandler<PlayingEventArgs> PlayingInfoChanged;

        public void InitContext(GlobalContext context)
        {
            
        }

        public void Init()
        {
            List<ProviderEntry> providers = new List<ProviderEntry>()
            {
                new ProviderEntry()
                {
                    Provider = new JsonApi(this),
                    Weight = 10
                },
                //new ProviderEntry()
                //{
                //    Provider = WebsocketApi.CreateWebsocketApi(),
                //    Weight = 100
                //}
            };

            _provider = null;

            try
            {
                _provider = providers
                    .Where(entry => entry.Provider.IsUseable())
                    .OrderByDescending(entry => entry.Weight)
                    .First()
                    .Provider;

                _provider.PlayingInfoChanged += ProviderOnPlayingInfoChanged;
                _provider.Start();
            }
            catch
            {
                _provider = null;
            }
        }

        private void ProviderOnPlayingInfoChanged(object sender, PlayingEventArgs e)
        {
            PlayingInfoChanged?.Invoke(this, e);
        }

        public Task<PlayingInfo> GetCurrent()
        {
            return _provider?.GetCurrent();
        }

        struct ProviderEntry
        {
            public IGpmdpProvider Provider;
            public int Weight;
        };

        private string _lastBitmapUrl;
        private BitmapRepresentation _lastBitmapRepresentation;
        public async Task<BitmapRepresentation> DownloadBitmap(string url)
        {
            if (_lastBitmapUrl != url)
            {
                DisposeHelper.DisposeAndNull(ref _lastBitmapRepresentation);

                _lastBitmapUrl = url;

                if (url == null)
                    return null;

                using (var client = new HttpClient())
            
                using (var stream = client.GetStreamAsync(url))
                using (var bitmap = (Bitmap) Bitmap.FromStream(await stream))

                    _lastBitmapRepresentation = new BitmapRepresentation(bitmap);
            }

            return _lastBitmapRepresentation;
        }
    }
}
