using System.Threading.Tasks;
using Vkm.Api.Basic;
using Vkm.Api.Service;

namespace Vkm.Library.Interfaces.Service.Player
{
    public interface IAlbumCoverService: IService
    {
        Task<BitmapRepresentation> GetCover(string artist, string album);
    }
}