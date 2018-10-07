using Vkm.Api.Identification;
using Vkm.Api.Module;

namespace Vkm.Api.Layout
{
    public interface ILayoutFactory: IModule
    {
        ILayout CreateLayout(Identifier id);
    }
}