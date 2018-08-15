using System.Collections.Generic;

namespace Vkm.Api.Module
{
    public interface IModulesService
    {
        IEnumerable<T> GetModules<T>() where T : IModule;
    }
}
