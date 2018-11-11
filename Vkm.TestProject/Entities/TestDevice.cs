using System;
using System.Collections.Generic;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Device;
using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.TestProject.Entities
{
    internal class TestDevice: IDevice
    {
        public Identifier Id { get; }

        public int DrawnCycles { get; private set; }

        public event EventHandler<IEnumerable<LayoutDrawElement>> BitmapsSet; 
        
        public void InitContext(GlobalContext context)
        {
            
        }

        public void Init()
        {
            
        }

        public void Dispose()
        {
            
        }

        public IconSize IconSize => new IconSize(10, 10);
        public DeviceSize ButtonCount => new DeviceSize(5,5);
        
        public void SetBitmaps(IEnumerable<LayoutDrawElement> elements)
        {
            DrawnCycles++;
            BitmapsSet?.Invoke(this, elements);
        }

        public void SetBrightness(byte valuePercent)
        {
            
        }

        public event EventHandler<ButtonEventArgs> ButtonEvent;

        public void PressButton(Location location, bool isDown)
        {
            ButtonEvent?.Invoke(this, new ButtonEventArgs(location, isDown));
        }
    }

    internal class TestDeviceFactory : IDeviceFactory
    {
        public Identifier Id => new Identifier("Test.DeviceFactory");
        public string Name => "Test Device Factory";
        
        public IDevice[] GetDevices()
        {
            return new[] {new TestDevice()};
        }
    }
}