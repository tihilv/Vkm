using System;
using System.Collections.Generic;
using System.Linq;
using OpenMacroBoard.SDK;
using StreamDeckSharp;
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
        private IconSize _iconSize;

        private IMacroBoard _device;

        public Identifier Id => _identifier;
        public IconSize IconSize => _iconSize;

        public DeviceSize ButtonCount
        {
            get
            {
                if (_device.Keys.Count == 6)
                    return new DeviceSize(3, 2);
                if (_device.Keys.Count == 15)
                    return new DeviceSize(5, 3);
                if (_device.Keys.Count == 32)
                    return new DeviceSize(8, 4);

                return new DeviceSize((byte)_device.Keys.Count, 1);
            }
        }

        public event EventHandler<ButtonEventArgs> ButtonEvent;

        public StreamDeckDevice(StreamDeckDeviceReference devicePath)
        {
            var device = devicePath.Open();
            _device = device.WithButtonPressEffect(new ButtonPressEffectConfig() {Scale = 0.9}).WithDisconnectReplay();
            _identifier = new Identifier("StreamDeck." + device.GetSerialNumber());
        }

        public void InitContext(GlobalContext context)
        {
            
        }

        public void Init()
        {
            _iconSize = new IconSize((ushort)_device.Keys.Max(k=>k.Width), (ushort)_device.Keys.Max(k=>k.Height));

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
                _device?.SetKeyBitmap(element.Location.X + element.Location.Y * ButtonCount.Width, FromBitmapRepresentation(element.BitmapRepresentation));
            }
        }

        private static KeyBitmap FromBitmapRepresentation(BitmapRepresentation representation)
        {
            int width = representation.Width;
            int height = representation.Height;

            byte[] bitmapData = new byte[width * height * 3];
            byte[] scan0 = representation.Bytes;

            var stride = scan0.Length / height;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int index3 = stride * y + x * 3;
                    int index4 = (width * y + x) * 3;
                    bitmapData[index4] = scan0[index3];
                    bitmapData[index4 + 1] = scan0[index3 + 1];
                    bitmapData[index4 + 2] = scan0[index3 + 2];
                }
            }

            return KeyBitmap.Create.FromBgr24Array(width, height, bitmapData);
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
