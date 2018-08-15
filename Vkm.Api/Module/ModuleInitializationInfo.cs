using System;
using Vkm.Api.Identification;

namespace Vkm.Api.Module
{
    [Serializable]
    public struct ModuleInitializationInfo
    {
        public readonly Identifier FactoryId;
        public readonly Identifier ChildId;

        public ModuleInitializationInfo(Identifier factoryId, Identifier childId)
        {
            FactoryId = factoryId;
            ChildId = childId;
        }
    }
}
