using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Library.LayoutSwitch
{
    public class LayoutSwitchLayoutFactory: ILayoutFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.DefaultLayoutSwitch.Factory");

        public Identifier Id => Identifier;

        public string Name => "Layout switch";

        public ILayout CreateLayout(Identifier id)
        {
            return new LayoutSwitchLayout(id);
        }
    }
}