//#define VIRTUAL
using System.Linq;
using Vkm.Api.Device;
using Vkm.Api.Identification;

namespace Vkm.Device.StreamDeck
{
    public class StreamDeckDeviceFactory : IDeviceFactory
    {
        public Identifier Id { get; private set; }
        public string Name => "Stream Deck Devices";

        public StreamDeckDeviceFactory()
        {
            Id = new Identifier("StreamDeck");
        }

        public IDevice[] GetDevices()
        {
#if VIRTUAL
            return new IDevice[] {new VirtualDevice()};
#else      
            return StreamDeckSharp.StreamDeck.EnumerateDevices().Select(d => new StreamDeckDevice(d.DevicePath)).ToArray();
#endif
        }

        

        
    }
}