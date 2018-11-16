using Vkm.Api.Basic;
using Vkm.Api.Identification;

namespace Vkm.Intercom.Service.Api
{
    interface IVkmRemoteService: IRemoteService
    {
        IntercomDeviceInfo[] GetDevices();

        Identifier GetLayout(string name);

        void RemoveLayout(Identifier layoutId);

        void SwitchToLayout(Identifier deviceId, Identifier layoutId);

        void SetBitmap(Identifier layoutId, Location location, byte[] bitmapBytes);

        void RemoveBitmap(Identifier layoutId, Location location);
    }
}