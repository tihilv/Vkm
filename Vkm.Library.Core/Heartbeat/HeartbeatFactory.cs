using Vkm.Api.Element;
using Vkm.Api.Identification;

namespace Vkm.Library.Heartbeat
{
    public class HeartbeatFactory : IElementFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.HeartbeatElement.Factory");

        public Identifier Id => Identifier;
        public string Name => "Heartbeat";

        public IElement CreateElement(Identifier id)
        {
            return new HeartbeatElement(id);
        }
    }
}