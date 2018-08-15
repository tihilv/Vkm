using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;

namespace Vkm.Library.CompositeLayout
{
    class CompositeLayoutFactory: ILayoutFactory
    {
        public Identifier Id => Identifiers.DefaultCompositeLayoutFactory;
        
        public string Name => "Composite Layouts";

        public ILayout CreateLayout(Identifier id)
        {
            return new CompositeLayout(id);
        }
    }
}
