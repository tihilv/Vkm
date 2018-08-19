using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using StreamDeckSharp;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Device;
using Vkm.Api.Identification;

namespace Vkm.Device.StreamDeck
{
    public class StreamDeckDevice: IDevice
    {
        private readonly Identifier _identifier;
        private readonly string _devicePath;
        private IconSize _iconSize;

        private IStreamDeck _device;

        public Identifier Id => _identifier;
        public IconSize IconSize => _iconSize;

        public DeviceSize ButtonCount
        {
            get
            {
                if (_device.KeyCount == 15)
                    return new DeviceSize(5, 3);

                return new DeviceSize((byte)_device.KeyCount, 1);
            }
        }

        public event EventHandler<ButtonEventArgs> ButtonEvent;

        public StreamDeckDevice(string devicePath)
        {
            _devicePath = devicePath;
            _identifier = new Identifier("StreamDeck." + _devicePath);
        }

        public void InitContext(GlobalContext context)
        {
            
        }

        public void Init()
        {
            _device = StreamDeckSharp.StreamDeck.OpenDevice(_devicePath);
            _iconSize = new IconSize(_device.IconSize, _device.IconSize);

            _device.KeyStateChanged += DeviceOnKeyStateChanged;
        }

        private void DeviceOnKeyStateChanged(object sender, KeyEventArgs e)
        {
            ButtonEvent?.Invoke(this, new ButtonEventArgs(new Location((byte)(ButtonCount.Width - e.Key % ButtonCount.Width - 1), (byte)(e.Key / ButtonCount.Width)), e.IsDown));
        }

        public void SetBitmap(Location location, Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);
                stream.Position = 0;
                _device.SetKeyBitmap(ButtonCount.Width - 1-location.X + location.Y * ButtonCount.Width, KeyBitmap.FromStream(stream));
            }
        }

        public void SetBrightness(byte valuePercent)
        {
            _device.SetBrightness(valuePercent);
        }

        public void Dispose()
        {
            _device.KeyStateChanged += DeviceOnKeyStateChanged;
            _device.Dispose();
        }
    }
}
