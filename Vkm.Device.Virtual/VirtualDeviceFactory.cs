#define VIRTUAL

using Vkm.Api.Device;
using Vkm.Api.Identification;

namespace Vkm.Device.Virtual
{
    public class StreamDeckDeviceFactory : IDeviceFactory
    {
        public Identifier Id { get; private set; }
        public string Name => "Virtual Devices";

        public StreamDeckDeviceFactory()
        {
            Id = new Identifier("StreamDeck");
        }

        public IDevice[] GetDevices()
        {
            return new IDevice[]
            {
#if VIRTUAL
                new VirtualDevice()
#endif
            };
        }
       
    }
}