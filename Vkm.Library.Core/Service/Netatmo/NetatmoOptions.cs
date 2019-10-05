using System;
using Vkm.Api.Options;

namespace Vkm.Library.Service.Netatmo
{
    [Serializable]
    public class NetatmoOptions : IOptions
    {
        private string _clientId;
        private string _secret;

        private string _login;
        private string _password;

        private TimeSpan _historyRefreshSpan;
        private int _maxMeasureCount;

        public string ClientId
        {
            get => _clientId;
            set => _clientId = value;
        }

        public string Secret
        {
            get => _secret;
            set => _secret = value;
        }

        public string Login
        {
            get => _login;
            set => _login = value;
        }

        public string Password
        {
            get => _password;
            set => _password = value;
        }

        public TimeSpan HistoryRefreshSpan
        {
            get => _historyRefreshSpan;
            set => _historyRefreshSpan = value;
        }

        public int MaxMeasureCount
        {
            get => _maxMeasureCount;
            set => _maxMeasureCount = value;
        }
    }
}