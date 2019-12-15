using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vkm.Api.Common;
using Vkm.Api.Configurator;
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
    public class GlobalContext: IOptionsProvider
    {
        public Identifier Id => new Identifier("CoreContext.Options");

        public event EventHandler<EventArgs> LayoutRemoved; 

        private readonly IDevice[] _devices;
        private GlobalOptions _globalOptions;
        private readonly ConcurrentDictionary<Identifier, ILayout> _layouts;
        private readonly ConcurrentDictionary<Identifier, ITransition> _transitions;
        private readonly ConcurrentDictionary<Identifier, IDeviceHook> _deviceHooks;
        private readonly LazyDictionary<Type, IService[]> _initedServices;
        private readonly ConcurrentDictionary<IService, IService> _initedServiceInstances;
        
        public GlobalOptions Options => _globalOptions;

        public GlobalServices Services { get; private set; }

        public IDevice[] Devices => _devices;
        public ConcurrentDictionary<Identifier, ILayout> Layouts => _layouts;
        public ConcurrentDictionary<Identifier, ITransition> Transitions => _transitions;
        public ConcurrentDictionary<Identifier, IDeviceHook> DeviceHooks => _deviceHooks;

        public GlobalContext(GlobalServices services)
        {
            Services = services;

            _initedServices = new LazyDictionary<Type, IService[]>();
            _initedServiceInstances = new ConcurrentDictionary<IService, IService>();
            
            _devices = services.ModulesService.GetModules<IDeviceFactory>().SelectMany(d => d.GetDevices()).ToArray();
            var configurators = services.ModulesService.GetModules<IConfigurator>().ToArray();
            var tempGlobalOptions = new GlobalOptions();
            foreach (IConfigurator configurator in configurators)
            {
                configurator.GlobalOptions = tempGlobalOptions;
                configurator.Devices = _devices;
            }

            services.OptionsService.InitOptions(configurators);
            services.OptionsService.InitEntity(this);
            
            _layouts = InitLayouts();
            _transitions = InitTransitions();
            _deviceHooks = InitDeviceHooks();
        }

        private ConcurrentDictionary<Identifier, ILayout> InitLayouts()
        {
            var result = new ConcurrentDictionary<Identifier, ILayout>();

            foreach (var initInfo in Options.LayoutLoadOptions.InitializationInfos)
            {
                var layout = CreateLayout(initInfo);
                if (layout != null)
                    result.TryAdd(initInfo.ChildId, layout);
            }

            return result;
        }

        private ConcurrentDictionary<Identifier, ITransition> InitTransitions()
        {
            var result = new ConcurrentDictionary<Identifier, ITransition>();

            Parallel.ForEach(Options.TransitionLoadOptions.InitializationInfos, (initInfo) => 
            {
                var transition = CreateTransition(initInfo);
                if (transition != null)
                    result.TryAdd(initInfo.ChildId, transition);
            });

            return result;
        }

        
        private ConcurrentDictionary<Identifier, IDeviceHook> InitDeviceHooks()
        {
            var result = new ConcurrentDictionary<Identifier, IDeviceHook>();

            foreach (var deviceHook in Services.ModulesService.GetModules<IDeviceHook>())
            {
                InitializeEntity(deviceHook);
                result[deviceHook.Id] = deviceHook;
            }

            return result;
        }


        private ITransition CreateTransition(ModuleInitializationInfo initInfo)
        {
            var layoutFactory = Services.ModulesService.GetModules<ITransitionFactory>().FirstOrDefault(f => f.Id == initInfo.FactoryId);
            if (layoutFactory != null)
            {
                var transition = layoutFactory.CreateTransition(initInfo.ChildId);
                InitializeEntity(transition);

                return transition;
            }

            return null;
        }

        private ILayout CreateLayout(ModuleInitializationInfo initInfo)
        {
            var layoutFactory = Services.ModulesService.GetModules<ILayoutFactory>().FirstOrDefault(f => f.Id == initInfo.FactoryId);
            if (layoutFactory != null)
            {
                try
                {
                    var layout = layoutFactory.CreateLayout(initInfo.ChildId);
                    InitializeEntity(layout);
                    return layout;
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.ToString());

                }
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
                var services = Services.ModulesService.GetModules<T>()
                    .Where(s => !Options.DiabledServices.Contains(s.Id)).Cast<IService>().ToArray();

                foreach (var service in services)
                    if (_initedServiceInstances.TryAdd(service, service))
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

        public IOptions GetDefaultOptions()
        {
            return new GlobalOptions();
        }

        public void InitOptions(IOptions options)
        {
            _globalOptions = (GlobalOptions) options;
        }

        public void RemoveLayout(Identifier layoutId)
        {
            Layouts.TryRemove(layoutId, out var layout);
            
            LayoutRemoved?.Invoke(layout, EventArgs.Empty);
        }
    }
}