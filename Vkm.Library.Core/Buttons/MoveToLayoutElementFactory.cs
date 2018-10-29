using Vkm.Api.Element;
using Vkm.Api.Identification;

namespace Vkm.Library.Buttons
{
    public class MoveToLayoutElementFactory : IElementFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.DefaultMoveToLayout.Factory");

        public Identifier Id => Identifier;

        public string Name => "Switches to Layouts";

        public IElement CreateElement(Identifier id)
        {
            return new MoveToLayoutElement(id);
        }
    }
}