using System;
using System.Collections.Generic;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Api.Device
{
    public interface IDevice: IIdentifiable, IInitializable, IDisposable
    {
        IconSize IconSize { get; }
        DeviceSize ButtonCount { get; }

        void SetBitmaps(IEnumerable<LayoutDrawElement> elements);
        void SetBrightness(byte valuePercent);

        event EventHandler<ButtonEventArgs> ButtonEvent;
    }
}
