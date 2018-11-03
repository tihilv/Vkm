using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Vkm.Api.Device;
using Vkm.Api.Element;
using Vkm.Api.Hook;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Module;
using Vkm.Api.Options;
using Vkm.Api.Service;
using Vkm.Api.Transition;

namespace Vkm.Api.Data
{
    public class GlobalContext
    {
        private readonly Func<IDevice[]> _devicesFunc;
        private readonly Func<ConcurrentDictionary<Identifier, ILayout>> _layoutsFunc;
        private readonly Func<ConcurrentDictionary<Identifier, ITransition>> _transitionsFunc;
        private readonly Func<ConcurrentDictionary<Identifier, IDeviceHook>> _deviceHooksFunc;
        private readonly ConcurrentDictionary<Type, IService[]> _initedServices;

        public GlobalOptions Options { get; private set; }

        public GlobalServices Services { get; private set; }

        public IDevice[] Devices => _devicesFunc();
        public ConcurrentDictionary<Identifier, ILayout> Layouts => _layoutsFunc();
        public ConcurrentDictionary<Identifier, ITransition> Transitions => _transitionsFunc();
        public ConcurrentDictionary<Identifier, IDeviceHook> DeviceHooks => _deviceHooksFunc();

        public GlobalContext(GlobalOptions globalOptions, GlobalServices services, Func<IDevice[]> devicesFunc, Func<ConcurrentDictionary<Identifier, ILayout>> layoutsFunc, Func<ConcurrentDictionary<Identifier, ITransition>> transitionsFunc, Func<ConcurrentDictionary<Identifier, IDeviceHook>> deviceHooksFunc)
        {
            Options = globalOptions;
            Services = services;

            _devicesFunc = devicesFunc;
            _layoutsFunc = layoutsFunc;
            _transitionsFunc = transitionsFunc;
            _deviceHooksFunc = deviceHooksFunc;

            _initedServices = new ConcurrentDictionary<Type, IService[]>();
        }

        public ITransition CreateTransition(ModuleInitializationInfo initInfo, Action<ITransition> beforeInit = null)
        {
            var layoutFactory = Services.ModulesService.GetModules<ITransitionFactory>().FirstOrDefault(f => f.Id == initInfo.FactoryId);
            if (layoutFactory != null)
            {
                var transition = layoutFactory.CreateTransition(initInfo.ChildId);
                InitializeEntity(transition, beforeInit);

                return transition;
            }

            return null;
        }

        public ILayout CreateLayout(ModuleInitializationInfo initInfo)
        {
            var layoutFactory = Services.ModulesService.GetModules<ILayoutFactory>().FirstOrDefault(f => f.Id == initInfo.FactoryId);
            if (layoutFactory != null)
            {
                var layout = layoutFactory.CreateLayout(initInfo.ChildId);
                InitializeEntity(layout);
                return layout;
            }

            return null;
        }

        public IElement CreateElement(ModuleInitializationInfo initInfo)
        {
            var layoutFactory = Services.ModulesService.GetModules<IElementFactory>().FirstOrDefault(f => f.Id == initInfo.FactoryId);
            if (layoutFactory != null)
            {
                var element = layoutFactory.CreateElement(initInfo.ChildId);
                InitializeEntity(element);
                return element;
            }

            return null;
        }

        public IEnumerable<T> GetServices<T>() where T : IService
        {
            return _initedServices.GetOrAdd(typeof(T), type =>
            {
                var services = Services.ModulesService.GetModules<T>().Where(s => !Options.DiabledServices.Contains(s.Id)).Cast<IService>().ToArray();
                foreach (var service in services)
                    InitializeEntity(service);

                return services;
            }).OfType<T>();
        }

        public T InitializeEntity<T>(T entity, Action<T> beforeInit = null)
        {
            (entity as IInitializable)?.InitContext(this);

            if (entity is IOptionsProvider optionsProvider)
                Services.OptionsService.InitEntity(optionsProvider);

            beforeInit?.Invoke(entity);
            
            (entity as IInitializable)?.Init();

            return entity;
        }

    }
}