using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Vkm.Api.Basic;
using Vkm.Api.Identification;

namespace Vkm.Library.Interfaces.Services
{
    public class CachedBitmapDownloader : IBitmapDownloadService
    {
        private const int CacheSize = 10;
        
        public Identifier Id => new Identifier("Vkm.BitmapDownloadService");
        public string Name => "Bitmap Download Service";

        private readonly ConcurrentDictionary<string, Task<BitmapRepresentation>> _cache;

        public CachedBitmapDownloader()
        {
            _cache = new ConcurrentDictionary<string, Task<BitmapRepresentation>>();
        }

        public async Task<BitmapRepresentation> GetBitmap(string url)
        {
            if (string.IsNullOrEmpty(url))
                return (BitmapRepresentation)null;

            var result = _cache.GetOrAdd(url, async s =>
            {
                using (var client = new HttpClient())

                using (var stream = client.GetStreamAsync(url))
                using (var bitmap = (Bitmap) Image.FromStream(await stream))

                    return new BitmapRepresentation(bitmap);
                
            });

            if (_cache.Count > CacheSize)
            {
                var victim = _cache.Keys.First(v => v != url);
                _cache.TryRemove(victim, out _);
            }
            
            return (await result).Clone();
        }
    }
}