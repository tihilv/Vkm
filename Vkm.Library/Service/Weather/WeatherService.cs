using System;
using System.Linq;
using System.Threading.Tasks;
using OpenWeatherMap;
using Vkm.Api.Identification;
using Vkm.Api.Options;
using Vkm.Library.Interfaces.Service.Weather;

namespace Vkm.Library.Service.Weather
{
    public sealed class OpenWeatherService : IWeatherService, IOptionsProvider
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.WeatherService");
        private OpenWeatherOptions _options;

        public Identifier Id => Identifier;
        public string Name => "OpenWeather service";

        public OpenWeatherService()
        {
        }

        public IOptions GetDefaultOptions()
        {
            return new OpenWeatherOptions();
        }

        public void InitOptions(IOptions options)
        {
            _options = (OpenWeatherOptions) options;
        }

        public async Task<WeatherInfo> GetCurrentWeather(string city)
        {
            var client = new OpenWeatherMapClient(_options.OpenWeatherApiKey);
            var currentWeather = await client.CurrentWeather.GetByName(city, MetricSystem.Metric).ConfigureAwait(false);
            return new WeatherInfo(
                currentWeather.LastUpdate.Value, 
                currentWeather.Temperature.Value,
                currentWeather.Temperature.Min,
                currentWeather.Temperature.Max,
                currentWeather.Pressure.Value,
                currentWeather.Humidity.Value,
                currentWeather.Clouds.Name,
                currentWeather.Weather.Number.ToString());
        }

        public async Task<WeatherInfo[]> GetForecast(string city, int dayCount)
        {
            var client = new OpenWeatherMapClient(_options.OpenWeatherApiKey);
            var forecast = await client.Forecast.GetByName(city, true, MetricSystem.Metric, count: 5).ConfigureAwait(false);
            return forecast.Forecast.Select(s => ToWeatherInfo(s, false)).ToArray();
        }

        public async Task<WeatherInfo[]> GetForecastForDay(string city, DateTime fromDate)
        {
            int[] hours = new[] {6, 12, 15, 18, 21};
            var client = new OpenWeatherMapClient(_options.OpenWeatherApiKey);
            var forecast = await client.Forecast.GetByName(city, false, MetricSystem.Metric).ConfigureAwait(false);

            var result = forecast.Forecast.Where(f =>
            {
                var from = DateTime.SpecifyKind(f.From, DateTimeKind.Utc);
                return from >= fromDate && from < fromDate.Date.AddDays(1) && (from.Hour >= 6 && from.Hour <= 22);
            });

            return result.Select(s=>ToWeatherInfo(s, true)).ToArray();
        }

        private WeatherInfo ToWeatherInfo(ForecastTime currentWeather, bool forTime)
        {
            return new WeatherInfo(
                forTime?currentWeather.From:currentWeather.Day, 
                currentWeather.Temperature.Value,
                currentWeather.Temperature.Min,
                currentWeather.Temperature.Max,
                currentWeather.Pressure.Value,
                currentWeather.Humidity.Value,
                currentWeather.Clouds.Value,
                currentWeather.Symbol.Number.ToString());
        }
    }
}
