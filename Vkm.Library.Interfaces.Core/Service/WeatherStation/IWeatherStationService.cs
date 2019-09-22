using System.Collections.Generic;
using System.Threading.Tasks;
using Vkm.Api.Service;

namespace Vkm.Library.Interfaces.Service.WeatherStation
{
    public interface IWeatherStationService: IService
    {
        Task<WeatherStationData> GetData();
        
        Task<IReadOnlyCollection<WeatherStationData>> GetHistorical();
    }
}
