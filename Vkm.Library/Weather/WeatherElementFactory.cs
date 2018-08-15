using Vkm.Api.Element;
using Vkm.Api.Identification;

namespace Vkm.Library.Weather
{
    public class WeatherElementFactory : IElementFactory
    {
        public static Identifier Identifier = new Identifier("Vkm.WeatherElement.Factory");
        public Identifier Id => Identifier;

        public string Name => "Weater Element";

        public IElement CreateElement(Identifier id)
        {
            return new WeatherElement(id);
        }
    }
}