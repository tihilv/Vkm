
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Netatmo;
using NodaTime;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Options;
using Vkm.Api.Time;
using Vkm.Library.Interfaces.Service.WeatherStation;

namespace Vkm.Library.Service.Netatmo
{
    public class NetatmoWeatherStationService: IWeatherStationService, IOptionsProvider, IInitializable
    {
        private const double PressureCoef = 1.33322;
        
        private const string TemperatureId = "Temperature";
        private const string HumidityId = "Humidity";
        
        public static readonly Identifier Identifier = new Identifier("Vkm.LastFmCoverService");

        private NetatmoOptions _options;
        private ITimerToken _netatmoTimerToken;

        private Queue<WeatherStationData> _historyData;
        private ITimerToken _historyTimerToken;

        private ITimerService _timerService;
       
        private IClient _netatmoClient;

        private Task _initializationTask;
        
        
        public Identifier Id => Identifier;
        public string Name => "Netatmo Weather Station Service";

        public IOptions GetDefaultOptions()
        {
            return new NetatmoOptions();
        }

        public void InitOptions(IOptions options)
        {
            _options = (NetatmoOptions)options;
        }

        public async Task<WeatherStationData> GetData()
        {
            await _initializationTask;
            return await GetDataInt();
        }

        private async Task<WeatherStationData> GetDataInt()
        {
            var stationsData = await _netatmoClient.Weather.GetStationsData();

            var device = stationsData.Body.Devices[0];
            List<WeatherStationDeviceData> devices = new List<WeatherStationDeviceData>();
            var mainDevice = new WeatherStationDeviceData(0, device.DashboardData.Temperature, device.DashboardData.HumidityPercent, (int)(device.DashboardData.Pressure*10/PressureCoef)/10.0 , device.DashboardData.CO2, device.DashboardData.Noise);
            devices.Add(mainDevice);

            var moduleId = 1;
            foreach (var module in device.Modules)
            {
                double? temp = null;
                if (module.DataType.Contains(TemperatureId))
                    temp = double.Parse(module.DashboardData[TemperatureId].ToString());
                
                int? humidity = null;
                if (module.DataType.Contains(HumidityId))
                    humidity = int.Parse(module.DashboardData[HumidityId].ToString());
                
                var moduleDevice = new WeatherStationDeviceData((byte)(moduleId++), temp, humidity);
                devices.Add(moduleDevice);
            }
            
            return new WeatherStationData(devices.ToArray());

        }
        
        public async Task<IReadOnlyCollection<WeatherStationData>> GetHistorical()
        {
            await _initializationTask;
            return _historyData;
        }

        public void InitContext(GlobalContext context)
        {
            _timerService = context.Services.TimerService;
        }

        private void RefreshToken()
        {
            _initializationTask = _netatmoClient.RefreshToken();
        }

        public void Init()
        {
            _initializationTask = InitClient();
        }

        private async Task InitClient()
        {
            _netatmoClient = new Client(SystemClock.Instance, " https://api.netatmo.com/", _options.ClientId, _options.Secret);
            await _netatmoClient.GenerateToken(_options.Login, _options.Password, new[] { Scope.StationRead });
            RegisterTokenRefresh();
            RegisterHistoryRefresh();
            await GetDataInt();
        }

        void RegisterTokenRefresh()
        {
            var token = _netatmoClient.CredentialManager.CredentialToken;
            var expirationTime = token.ExpiresAt.ToDateTimeUtc();
            var timeSpan = (expirationTime - DateTime.UtcNow).Add(TimeSpan.FromSeconds(-20));
            _netatmoTimerToken = _timerService.RegisterTimer(timeSpan, RefreshToken, true);
            _netatmoTimerToken.Start();
        }

        private void RegisterHistoryRefresh()
        {
            _historyData = new Queue<WeatherStationData>();
            RegisterHistoryValue();
            _historyTimerToken = _timerService.RegisterTimer(_options.HistoryRefreshSpan, RegisterHistoryValue);
            _historyTimerToken.Start();
        }

        private async void RegisterHistoryValue()
        {
            try
            {
                _historyData.Enqueue(await GetData());

                while (_historyData.Count > _options.MaxMeasureCount)
                    _historyData.Dequeue();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Netatmo service error: {ex}");
            }
        }
    }
}