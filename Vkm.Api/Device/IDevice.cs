using System;
using System.Drawing;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Identification;

namespace Vkm.Api.Device
{
    public interface IDevice: IIdentifiable, IInitializable, IDisposable
    {
        IconSize IconSize { get; }
        DeviceSize ButtonCount { get; }

        void SetBitmap(Location location, Bitmap bitmap);
        void SetBrightness(byte valuePercent);
        void Clear();

        event EventHandler<ButtonEventArgs> ButtonEvent;
    }
}
