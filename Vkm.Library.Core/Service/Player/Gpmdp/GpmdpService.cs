using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Library.Interfaces.Service.Player;
using Vkm.Library.Interfaces.Services;
using Vkm.Library.Service.Player.Gpmdp.Providers;

namespace Vkm.Library.Service.Player.Gpmdp
{
    public class GpmdpService : IPlayerService, IInitializable
    {
        private IBitmapDownloadService _bitmapDownloadService;
        private IGpmdpProvider _provider;

        public Identifier Id => new Identifier("Vkm.GpmdpService");
        public string Name => "Google Play Music Service";

        public event EventHandler<PlayingEventArgs> PlayingInfoChanged;

        public void InitContext(GlobalContext context)
        {
            _bitmapDownloadService = context.GetServices<IBitmapDownloadService>().FirstOrDefault();
        }

        public void Init()
        {
            List<ProviderEntry> providers = new List<ProviderEntry>()
            {
                new ProviderEntry()
                {
                    Provider = new JsonApi(_bitmapDownloadService),
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
                    .FirstOrDefault().Provider;

                if (_provider != null)
                {
                    _provider.PlayingInfoChanged += ProviderOnPlayingInfoChanged;
                    _provider.Start();
                }
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
            if (_provider == null)
                return Task.FromResult<PlayingInfo>(null);

            return _provider.GetCurrent();
        }

        struct ProviderEntry
        {
            public IGpmdpProvider Provider;
            public int Weight;
        };
    }
}
