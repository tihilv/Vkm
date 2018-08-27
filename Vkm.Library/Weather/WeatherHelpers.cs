using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using OpenWeatherMap;
using Vkm.Library.Common;

namespace Vkm.Library.Weather
{
    class WeatherHelpers
    {
        internal static readonly FontFamily WeatherFontFamily;

        private static readonly Dictionary<string, string> _iconsDictionary;

        static WeatherHelpers()
        {
            WeatherFontFamily = FontService.Instance.GetFontFamilyByResourceName("Vkm.Library.Resources.weathericons-regular-webfont.ttf");

            _iconsDictionary = GetIconsDictionary();
        }

        private static Dictionary<string, string> GetIconsDictionary()
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

        public static string TempToStr(double temp)
        {
            int temperature = (int) temp;
            var temperatureStr = (temperature < 0) ? temperature.ToString() : $"+{temperature}";
            return temperatureStr;
        }

        public static string GetWeatherSymbol(string weatherSymbol)
        {
            var icon = "wi_owm_" + weatherSymbol;

            return _iconsDictionary[icon];
        }

        public string GetWeatherSymbol(Symbol symbol)
        {
            var icon = "wi_owm_" + symbol.Number;

            return _iconsDictionary[icon];
        }
    }
}