using System;
using Vkm.Api.Basic;
using Vkm.Api.Identification;
using Vkm.Api.Options;

namespace Vkm.Library.Hook
{
    [Serializable]
    public class LongPressHookOptions : IOptions
    {
        private Location _location;
        private Identifier _layoutIdentifier;

        public Location Location
        {
            get => _location;
            set => _location = value;
        }

        public Identifier LayoutIdentifier
        {
            get => _layoutIdentifier;
            set => _layoutIdentifier = value;
        }
    }
}