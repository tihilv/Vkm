using System.Threading.Tasks;
using Vkm.Api.Basic;

namespace Vkm.Library.Service.Player.Gpmdp.Providers
{
    interface IBitmapDownloader
    {
        Task<BitmapRepresentation> DownloadBitmap(string url);
    }
}
