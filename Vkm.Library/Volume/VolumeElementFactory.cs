using Vkm.Api.Element;
using Vkm.Api.Identification;

namespace Vkm.Library.Volume
{
    public class VolumeElementFactory: IElementFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.VolumeElement.Factory");

        public Identifier Id => Identifier;
        
        public string Name => "Volume Controls";

        public IElement CreateElement(Identifier id)
        {
            return new VolumeElement(id);
        }
    }
}