using System;
using System.Drawing;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Device;
using Vkm.Api.Identification;

namespace Vkm.Device.StreamDeck
{
    class VirtualDevice: IDevice
    {
        private VirtualDeviceForm _virtualDeviceForm;

        public Identifier Id => new Identifier("Virtual");

        public event EventHandler<ButtonEventArgs> ButtonEvent;

        public void InitContext(GlobalContext context)
        {
            
        }

        public void Init()
        {
            _virtualDeviceForm = new VirtualDeviceForm();
            _virtualDeviceForm.Init(ButtonCount, IconSize);
            _virtualDeviceForm.ButtonEvent += VirtualDeviceFormOnButtonEvent;
            _virtualDeviceForm.Show();
        }

        private void VirtualDeviceFormOnButtonEvent(object sender, ButtonEventArgs e)
        {
            ButtonEvent?.Invoke(this, e);
        }

        public void Dispose()
        {
            
        }

        public IconSize IconSize => new IconSize(72*2, 72*2);

        public DeviceSize ButtonCount => new DeviceSize(5, 3);
        
        public void SetBitmap(Location location, Bitmap bitmap)
        {
            _virtualDeviceForm.SetImage(location, bitmap);
        }

        public void SetBrightness(byte valuePercent)
        {
            
        }

        public void Clear()
        {
            _virtualDeviceForm.ClearImages();
        }
    }
}
