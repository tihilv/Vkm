using System.Collections.Generic;
using System.Linq;
using Vkm.Api.Module;

namespace Vkm.TestProject.Entities
{
    internal class TestModulesService: IModulesService
    {
        private readonly IModule[] _allModules;

        public TestModulesService()
        {
            _allModules = new IModule[]{new TestDeviceFactory(), new TestService(), };
        }

        public IEnumerable<T> GetModules<T>() where T : IModule
        {
            return _allModules.OfType<T>();
        }
    }
}