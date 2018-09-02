using Vkm.Api.Element;
using Vkm.Api.Identification;

namespace Vkm.Library.Power
{
    class PowerElementFactory: IElementFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.PowerElement.Factory");

        public Identifier Id => Identifier;
        public string Name => "Power";

        public IElement CreateElement(Identifier id)
        {
            return new PowerElement(id);
        }
    }
}
