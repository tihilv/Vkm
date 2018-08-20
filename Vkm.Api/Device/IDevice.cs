using System;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Identification;

namespace Vkm.Api.Device
{
    public interface IDevice: IIdentifiable, IInitializable, IDisposable
    {
        IconSize IconSize { get; }
        DeviceSize ButtonCount { get; }

        void SetBitmap(Location location, BitmapRepresentation bitmapRepresentation);
        void SetBrightness(byte valuePercent);

        event EventHandler<ButtonEventArgs> ButtonEvent;
    }
}
