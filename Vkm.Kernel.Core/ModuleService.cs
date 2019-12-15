using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Vkm.Api.Module;

namespace Vkm.Kernel
{
    public class ModulesService : IModulesService
    {
        private readonly List<IModule> _modules;

        public ModulesService(string path)
        {
            _modules = new List<IModule>();

            foreach (var filename in Directory.EnumerateFiles(path, "*.vkmext.dll", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    _modules.AddRange(LoadModules(filename));
                }
                catch
                {
                    Debug.WriteLine($"Module cannot be loaded from file '{filename}'.");
                }

            }
        }

        private IEnumerable<IModule> LoadModules(string path)
        {
            Assembly assembly = Assembly.LoadFile(path);
            Type[] types = assembly.GetTypes();
            var modelElementTypes = types.Where(t=>t.GetInterface(typeof(IModule).Name) != null && !t.ContainsGenericParameters);
            foreach (var modelElementType in modelElementTypes)
            {
                IModule module = null;

                ConstructorInfo constructorInfo = modelElementType.GetConstructor(new Type[] { });
                if (constructorInfo != null)
                {
                    module = (IModule) constructorInfo.Invoke(new object[] { });
                }

                if (module != null)
                {
                    Debug.WriteLine($"Module '{module.Name}' from file '{Path.GetFileName(path)}' is loaded.");
                    yield return module;
                }
            }
        }

        public IEnumerable<T> GetModules<T>() where T : IModule
        {
            return _modules.OfType<T>();
        }
    }
}
