using System;
using System.Collections.Generic;
using Vkm.Api.Module;

namespace Vkm.Api.Options
{
    [Serializable]
    public class LayoutLoadOptions: IOptions
    {
        private readonly List<ModuleInitializationInfo> _initializationInfos;

        public List<ModuleInitializationInfo> InitializationInfos => _initializationInfos;

        public LayoutLoadOptions()
        {
            _initializationInfos = new List<ModuleInitializationInfo>();
        }
    }
}
