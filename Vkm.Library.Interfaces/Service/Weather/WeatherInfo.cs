using System;

namespace Vkm.Library.Interfaces.Service.Weather
{
    public class WeatherInfo
    {
        public DateTime DateTime { get; private set; }
        public double? TemperatureCelsius { get; private set; }
        public double? TemperatureMinCelsius { get; private set; }
        public double? TemperatureMaxCelsius { get; private set; }
        public double? PressureMPa { get; private set; }
        public int? Humidity { get; private set; }
        public string Clouds { get; private set; }
        public string Symbol { get; private set; }

        public WeatherInfo(DateTime dateTime, double? temperatureCelsius, double? temperatureMinCelsius, double? temperatureMaxCelsius, double? pressureMPa, int? humidity, string clouds, string symbol)
        {
            DateTime = dateTime;
            TemperatureCelsius = temperatureCelsius;
            PressureMPa = pressureMPa;
            Symbol = symbol;
            Humidity = humidity;
            Clouds = clouds;
            TemperatureMinCelsius = temperatureMinCelsius;
            TemperatureMaxCelsius = temperatureMaxCelsius;
        }
    }
}