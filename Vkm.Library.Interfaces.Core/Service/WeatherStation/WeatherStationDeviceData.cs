namespace Vkm.Library.Interfaces.Service.WeatherStation
{
    public class WeatherStationDeviceData
    {
        public readonly byte Channel;
        public readonly double? Temperature;
        public readonly int? HumidityPercent;
        public readonly double? Pressure;
        public readonly int? Co2Measure;
        public readonly double? Noise;

        public WeatherStationDeviceData(byte channel, double? temperature = null, int? humidityPercent = null, double? pressure = null, int? co2Measure = null, double? noise = null)
        {
            Channel = channel;
            Noise = noise;
            Temperature = temperature;
            HumidityPercent = humidityPercent;
            Pressure = pressure;
            Co2Measure = co2Measure;
        }
    }
}