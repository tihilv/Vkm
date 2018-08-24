using Vkm.Api.Element;
using Vkm.Api.Identification;

namespace Vkm.Library.Date
{
    class DateElementFactory: IElementFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.DateElement.Factory");

        public Identifier Id => Identifier;
        public string Name => "Date";

        public IElement CreateElement(Identifier id)
        {
            return new DateElement(id);
        }
    }
}
