using System;
using Vkm.Api.Basic;
using Vkm.Library.Interfaces.Service.WeatherStation;

namespace Vkm.Library.WeatherStation
{
    internal class WeatherStationDataSource
    {
        private readonly Location _location;
        private readonly Func<WeatherStationData, double> _getFunc;
        private readonly Func<double, string> _transformFunc;
        private readonly string _suffix;

        public Location Location => _location;

        public Func<WeatherStationData, double> GetFunc => _getFunc;

        public Func<double, string> TransformFunc => _transformFunc;

        public string Suffix => _suffix;

        public WeatherStationDataSource(Location location, Func<WeatherStationData, double> func, Func<double, string> transformFunc, string suffix)
        {
            _location = location;
            _getFunc = func;
            _suffix = suffix;
            _transformFunc = transformFunc;
        }
    }
}