using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;

namespace Vkm.Library.Numpad
{
    class NumpadLayoutFactory: ILayoutFactory
    {
        public Identifier Id => Identifiers.DefaultNumpadFactory;

        public string Name => "Numpad";

        public ILayout CreateLayout(Identifier id)
        {
            return new NumpadLayout(id);
        }
    }
}
