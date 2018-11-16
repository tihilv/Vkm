using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Vkm.Intercom.Channels
{
    internal class TypeInfoProvider
    {
        private readonly Type _serviceType;
        
        private readonly ConcurrentDictionary<string, MethodInfo> _methods;
        private readonly ConcurrentDictionary<string, bool> _isOneWay;

        public TypeInfoProvider(Type serviceType)
        {
            _serviceType = serviceType;
            _methods = new ConcurrentDictionary<string, MethodInfo>();
            _isOneWay = new ConcurrentDictionary<string, bool>();
        }

        
        public TypeInfoProvider(IRemoteService service): this(service.GetType())
        {
        }
        
        internal MethodInfo GetMethodInfo(string name)
        {
            return _methods.GetOrAdd(name,
                n =>
                {
                    return _serviceType.GetMethods(BindingFlags.Instance | BindingFlags.Public).First(m => m.Name == n);
                });
        }
        
        internal bool GetIsOneWayMethod(string name)
        {
            return _isOneWay.GetOrAdd(name, n =>
            {
                var fromInterface = _serviceType.GetInterfaces().Any(i => i.GetMethod(name, BindingFlags.Instance | BindingFlags.Public)?.GetCustomAttribute<OneWayAttribute>() != null);
                return fromInterface || GetMethodInfo(n).GetCustomAttribute<OneWayAttribute>() != null;
            });
        }
    }
}