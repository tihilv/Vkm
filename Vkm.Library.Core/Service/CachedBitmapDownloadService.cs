using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Vkm.Api.Basic;
using Vkm.Api.Common;
using Vkm.Api.Identification;
using Vkm.Library.Interfaces.Services;

namespace Vkm.Library.Service
{
    public class CachedBitmapDownloader : IBitmapDownloadService
    {
        private const int CacheSize = 10;
        
        public Identifier Id => new Identifier("Vkm.BitmapDownloadService");
        public string Name => "Bitmap Download Service";

        private readonly LazyDictionary<string, Task<BitmapRepresentation>> _cache;

        public CachedBitmapDownloader()
        {
            _cache = new LazyDictionary<string, Task<BitmapRepresentation>>();
        }

        public async Task<BitmapRepresentation> GetBitmap(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

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

        public async Task<BitmapRepresentation> GetBitmapForExecutable(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return (BitmapRepresentation)null;

            var result = _cache.GetOrAdd(filePath, async s =>
            {
                using (var icon = Icon.ExtractAssociatedIcon(filePath))
                using (var iconBmp = icon.ToBitmap())
                    return new BitmapRepresentation(iconBmp);
                
            });

            if (_cache.Count > CacheSize)
            {
                var victim = _cache.Keys.First(v => v != filePath);
                _cache.TryRemove(victim, out _);
            }
            
            return (await result).Clone();
        }
    }
}