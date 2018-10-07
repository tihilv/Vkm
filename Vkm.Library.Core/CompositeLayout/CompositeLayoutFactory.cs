using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Library.CompositeLayout
{
    public class CompositeLayoutFactory: ILayoutFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.DefaultCompositeLayout.Factory");

        public Identifier Id => Identifier;
        
        public string Name => "Composite Layouts";

        public ILayout CreateLayout(Identifier id)
        {
            return new CompositeLayout(id);
        }
    }
}
