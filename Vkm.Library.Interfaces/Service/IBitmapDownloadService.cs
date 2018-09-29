using System.Threading.Tasks;
using Vkm.Api.Basic;
using Vkm.Api.Service;

namespace Vkm.Library.Interfaces.Services
{
    public interface IBitmapDownloadService: IService
    {
        Task<BitmapRepresentation> GetBitmap(string url);
    }
}