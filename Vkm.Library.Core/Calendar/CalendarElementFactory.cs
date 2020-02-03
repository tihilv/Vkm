using Vkm.Api.Element;
using Vkm.Api.Identification;

namespace Vkm.Library.Calendar
{
    public class CalendarElementFactory: IElementFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.CalendarElement.Factory");

        public Identifier Id => Identifier;
        
        public string Name => "Calendar Controls";

        public IElement CreateElement(Identifier id)
        {
            return new CalendarElement(id);
        }
    }
}