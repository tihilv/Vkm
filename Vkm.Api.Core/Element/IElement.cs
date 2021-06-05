using Vkm.Api.Basic;
using Vkm.Api.Drawable;

namespace Vkm.Api.Element
{
    public interface IElement: IDrawable
    {
        DeviceSize ButtonCount { get; }
    }
}