using Vkm.Api.Element;
using Vkm.Api.Identification;

namespace Vkm.Library.AudioSelect
{
    class AudioSelectElementFactory: IElementFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.AudioSelectElement.Factory");

        public Identifier Id => Identifier;
        public string Name => "Audio Select";

        public IElement CreateElement(Identifier id)
        {
            return new AudioSelectElement(id);
        }
    }
}
