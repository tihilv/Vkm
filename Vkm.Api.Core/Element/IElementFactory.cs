using Vkm.Api.Identification;
using Vkm.Api.Module;

namespace Vkm.Api.Element
{
    public interface IElementFactory: IModule
    {
        IElement CreateElement(Identifier id);
    }
}