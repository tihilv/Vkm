using Vkm.Api.Element;
using Vkm.Api.Identification;

namespace Vkm.Library.WeatherStation
{
    public class WeatherStationFactory : IElementFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.WeatherStation.Factory");

        public Identifier Id => Identifier;
        
        public string Name => "Weather Station Controls";

        public IElement CreateElement(Identifier id)
        {
            return new WeatherStationElement(id);
        }
    }
}