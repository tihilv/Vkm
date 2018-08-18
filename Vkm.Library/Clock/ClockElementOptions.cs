using System;
using Vkm.Api.Identification;
using Vkm.Api.Options;

namespace Vkm.Library.Clock
{
    [Serializable]
    internal class ClockElementOptions : IOptions
    {
        private Identifier _timerLayoutIdentifier;

        public Identifier TimerLayoutIdentifier
        {
            get => _timerLayoutIdentifier;
            set => _timerLayoutIdentifier = value;
        }
    }
}