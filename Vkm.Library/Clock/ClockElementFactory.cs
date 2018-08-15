using Vkm.Api.Element;
using Vkm.Api.Identification;

namespace Vkm.Library.Clock
{
    class ClockElementFactory: IElementFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.ClockElement.Factory");

        public Identifier Id => Identifier;
        public string Name => "Clocks";

        public IElement CreateElement(Identifier id)
        {
            return new ClockElement(id);
        }
    }
}
