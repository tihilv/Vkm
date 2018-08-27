using System;
using System.Threading.Tasks;
using Vkm.Api.Service;

namespace Vkm.Library.Interfaces.Service.Player
{
    public interface IPlayerService: IService
    {
        event EventHandler<PlayingEventArgs> PlayingInfoChanged;

        Task<PlayingInfo> GetCurrent();

    }
}
