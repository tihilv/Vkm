using System;
using System.Threading.Tasks;
using Vkm.Library.Interfaces.Service.Player;

namespace Vkm.Library.Service.Player.Gpmdp.Providers
{
    interface IGpmdpProvider
    {
        event EventHandler<PlayingEventArgs> PlayingInfoChanged; 

        void Start();

        bool IsUseable();

        Task<PlayingInfo> GetCurrent();
    }
}
