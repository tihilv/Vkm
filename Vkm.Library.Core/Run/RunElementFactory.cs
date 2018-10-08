using Vkm.Api.Element;
using Vkm.Api.Identification;

namespace Vkm.Library.Run
{
    public class RunElementFactory: IElementFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.RunElement.Factory");

        public Identifier Id => Identifier;
        public string Name => "Start Process";

        public IElement CreateElement(Identifier id)
        {
            return new RunElement(id);
        }
    }
}