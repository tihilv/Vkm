﻿using System;
using Vkm.Api.Options;

namespace Vkm.Library.Weather
{
    [Serializable]
    class WeatherOptions : IOptions
    {
        private string _place;

        public string Place
        {
            get => _place;
            set => _place = value;
        }
    }
}