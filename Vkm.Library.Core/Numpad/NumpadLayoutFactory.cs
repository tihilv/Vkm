using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Library.Numpad
{
    public class NumpadLayoutFactory: ILayoutFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.DefaultNumpad.Factory");

        public Identifier Id => Identifier;

        public string Name => "Numpad";

        public ILayout CreateLayout(Identifier id)
        {
            return new NumpadLayout(id);
        }
    }
}
