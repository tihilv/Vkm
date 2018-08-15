using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenWeatherMap;
using Vkm.Library.Common;

namespace Vkm.Library.Weather
{
    sealed class WeatherService
    {
        public static readonly WeatherService Instance = new WeatherService();

        private readonly Dictionary<string, string> _iconsDictionary;

        private FontFamily _weatherFontFamily;

        public FontFamily WeatherFontFamily => _weatherFontFamily;

        private WeatherService()
        {
            _weatherFontFamily = FontService.Instance.GetFontFamilyByResourceName("Vkm.Library.Resources.weathericons-regular-webfont.ttf");
            _iconsDictionary = GetIconsDictionary();
        }

        private Dictionary<string, string> GetIconsDictionary()
        {
            var result = new Dictionary<string, string>();

            string resource = "Vkm.Library.Resources.weathericons.xml";
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
            {
                XDocument doc = XDocument.Load(stream);
                foreach (var element in doc.Root.Elements())
                {
                    var key = element.Attribute("name").Value;
                    var value = element.Value;

                    result.Add(key, value);
                }
            }
            
            return result;
        }

        public async Task<CurrentWeatherResponse> GetWeather(string apiKey, string city)
        {
            var client = new OpenWeatherMapClient(apiKey);
            var currentWeather = await client.CurrentWeather.GetByName(city, MetricSystem.Metric).ConfigureAwait(false);
            return currentWeather;
        }

        public async Task<ForecastResponse> GetForecast(string apiKey, string city, int dayCount)
        {
            var client = new OpenWeatherMapClient(apiKey);
            var forecast = await client.Forecast.GetByName(city, true, MetricSystem.Metric, count:5).ConfigureAwait(false);
            return forecast;
        }

        public async Task<ForecastTime[]> GetForecastForDay(string apiKey, string city, DateTime fromDate)
        {
            int[] hours = new[] {6, 12, 15, 18, 21};
            var client = new OpenWeatherMapClient(apiKey);
            var forecast = await client.Forecast.GetByName(city, false, MetricSystem.Metric).ConfigureAwait(false);
            
            var result = forecast.Forecast.Where(f=>
            {
                var from = DateTime.SpecifyKind(f.From, DateTimeKind.Utc);
                return from >= fromDate && from < fromDate.Date.AddDays(1) && (from.Hour >= 6 && from.Hour <= 22);
            }).ToArray();

            return result;
        }

        public string GetWeatherSymbol(OpenWeatherMap.Weather weather)
        {
            var icon = "wi_owm_"+weather.Number;

            return _iconsDictionary[icon];
        }

        public string GetWeatherSymbol(Symbol symbol)
        {
            var icon = "wi_owm_"+symbol.Number;

            return _iconsDictionary[icon];
        }

        public string TempToStr(double temp)
        {
            int temperature = (int) temp;
            var temperatureStr = (temperature < 0) ? temperature.ToString() : $"+{temperature}";
            return temperatureStr;
        }

    }
}
