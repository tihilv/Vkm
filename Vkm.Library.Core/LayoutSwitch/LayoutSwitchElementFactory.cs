using Vkm.Api.Element;
using Vkm.Api.Identification;

namespace Vkm.Library.LayoutSwitch
{
    public class LayoutSwitchElementFactory: IElementFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.LayoutSwitchElement.Factory");

        public Identifier Id => Identifier;
        
        public string Name => "Layout Switches";

        public IElement CreateElement(Identifier id)
        {
            return new LayoutSwitchElement(id);
        }
    }
}