using Vkm.Api.Basic;
using Vkm.Api.Element;

namespace Vkm.Api.Layout
{
    public struct ElementPlacement
    {
        public readonly Location Location;
        public readonly IElement Element;

        public ElementPlacement(Location location, IElement element)
        {
            Location = location;
            Element = element;
        }
    }
}