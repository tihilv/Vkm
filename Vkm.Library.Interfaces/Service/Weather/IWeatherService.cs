using System;
using System.Threading.Tasks;
using Vkm.Api.Service;

namespace Vkm.Library.Interfaces.Service.Weather
{
    public interface IWeatherService: IService
    {
        Task<WeatherInfo> GetCurrentWeather(string city);
        Task<WeatherInfo[]> GetForecast(string city, int dayCount);
        Task<WeatherInfo[]> GetForecastForDay(string city, DateTime fromDate);
    }
}
