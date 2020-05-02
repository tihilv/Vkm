using Vkm.Api.Element;
using Vkm.Api.Identification;

namespace Vkm.Library.AudioSessions
{
    public class AudioSessionsElementFactory: IElementFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.AudioSessionsElement.Factory");

        public Identifier Id => Identifier;
        public string Name => "Audio Sessions";

        public IElement CreateElement(Identifier id)
        {
            return new AudioSessionsElement(id);
        }
    }
}
