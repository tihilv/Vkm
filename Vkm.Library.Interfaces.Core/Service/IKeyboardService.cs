using Vkm.Api.Service;

namespace Vkm.Library.Interfaces.Service
{
    public interface IKeyboardService: IService
    {
        void SendKeys(string keys);
        void SendKeys(byte keys);

        byte PreviousTrack { get; }
        byte NextTrack { get; }
        byte PlayPause { get; }
    }
}
