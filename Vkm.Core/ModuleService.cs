using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Vkm.Api.Module;

namespace Vkm.Core
{
    public class ModulesService : IModulesService
    {
        private readonly List<IModule> _modules;

        public ModulesService(string path)
        {
            _modules = new List<IModule>();

            AppDomain.CurrentDomain.AppendPrivatePath(path);

            foreach (var filename in Directory.EnumerateFiles(path, "*.dll"))
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

                string filename = Path.GetFileName(path);
                if (module != null)
                {
                    Debug.WriteLine($"Module '{module.Name}' from file '{filename}' is loaded.");
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
