using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using OpenMacroBoard.SDK;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Device;
using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Device.StreamDeck
{
    public class StreamDeckDevice: IDevice
    {
        private readonly Identifier _identifier;
        private readonly IDeviceReferenceHandle _devicePath;
        private IconSize _iconSize;

        private IMacroBoard _device;

        public Identifier Id => _identifier;
        public IconSize IconSize => _iconSize;

        public DeviceSize ButtonCount
        {
            get
            {
                if (_device.Keys.Count == 15)
                    return new DeviceSize(5, 3);

                return new DeviceSize((byte)_device.Keys.Count, 1);
            }
        }

        public event EventHandler<ButtonEventArgs> ButtonEvent;

        public StreamDeckDevice(IDeviceReferenceHandle devicePath)
        {
            _devicePath = devicePath;
            _identifier = new Identifier("StreamDeck." + _devicePath);
        }

        public void InitContext(GlobalContext context)
        {
            
        }

        public void Init()
        {
            _device = _devicePath.Open();
            _iconSize = new IconSize(_device.Keys.Max(k=>k.Width), _device.Keys.Max(k=>k.Width));

            _device.KeyStateChanged += DeviceOnKeyStateChanged;
        }

        private void DeviceOnKeyStateChanged(object sender, KeyEventArgs e)
        {
            ButtonEvent?.Invoke(this, new ButtonEventArgs(new Location((byte)(e.Key % ButtonCount.Width), (byte)(e.Key / ButtonCount.Width)), e.IsDown));
        }

        public void SetBitmaps(IEnumerable<LayoutDrawElement> elements)
        {
            foreach (var element in elements)
            {
                using (var bitmap = element.BitmapRepresentation.CreateBitmap())
                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Bmp);
                    stream.Position = 0;
                    _device?.SetKeyBitmap(element.Location.X + element.Location.Y * ButtonCount.Width, KeyBitmap.Create.FromStream(stream));
                }
            }
        }

        public void SetBrightness(byte valuePercent)
        {
            _device.SetBrightness(valuePercent);
        }

        public void Dispose()
        {
            _device.KeyStateChanged += DeviceOnKeyStateChanged;
            DisposeHelper.DisposeAndNull(ref _device);
        }
    }
}
