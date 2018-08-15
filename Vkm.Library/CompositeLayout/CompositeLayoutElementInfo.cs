using System;
using Vkm.Api.Basic;
using Vkm.Api.Module;

namespace Vkm.Library.CompositeLayout
{
    [Serializable]
    public class CompositeLayoutElementInfo
    {
        public ModuleInitializationInfo ModuleInfo { get; set; }

        public Location Location { get; set; }
    }
}