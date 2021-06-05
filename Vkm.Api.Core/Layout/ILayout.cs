using Vkm.Api.Drawable;

namespace Vkm.Api.Layout
{
    public interface ILayout: IDrawable
    {
        byte? PreferredBrightness { get; }
    }
}
