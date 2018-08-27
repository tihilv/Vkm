using System;
using Vkm.Api.Options;

namespace Vkm.Library.Service.Weather
{
    [Serializable]
    public class OpenWeatherOptions: IOptions
    {
        private string _openWeatherApiKey;

        public string OpenWeatherApiKey
        {
            get => _openWeatherApiKey;
            set => _openWeatherApiKey = value;
        }
    }
}