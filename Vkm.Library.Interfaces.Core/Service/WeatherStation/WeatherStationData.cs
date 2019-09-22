namespace Vkm.Library.Interfaces.Service.WeatherStation
{
    public class WeatherStationData
    {
        public readonly WeatherStationDeviceData[] Devices;

        public WeatherStationData(WeatherStationDeviceData[] devices)
        {
            Devices = devices;
        }
    }
}