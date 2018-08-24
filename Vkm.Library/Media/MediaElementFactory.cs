using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Library.Clock;

namespace Vkm.Library.Media
{
    class MediaElementFactory: IElementFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.MediaElement.Factory");

        public Identifier Id => Identifier;
        public string Name => "Media";

        public IElement CreateElement(Identifier id)
        {
            return new MediaElement(id);
        }
    }
}
