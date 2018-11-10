﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vkm.Api.Configurator;
using Vkm.Api.Data;
using Vkm.Api.Device;
using Vkm.Api.Hook;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Api.Transition;

namespace Vkm.Kernel
{
    public partial class VkmKernel: IOptionsProvider, IDisposable
    {
        private readonly GlobalContext _globalContext;

        private readonly ConcurrentDictionary<Identifier, DeviceManager> _deviceManagers;
        private readonly ConcurrentDictionary<Identifier, ILayout> _layouts;
        private readonly ConcurrentDictionary<Identifier, ITransition> _transitions;
        private readonly ConcurrentDictionary<Identifier, IDeviceHook> _deviceHooks;

        public VkmKernel()
        {
            var assemblyLocation = Path.GetDirectoryName(typeof(VkmKernel).Assembly.Location);
            var settingsLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vkm");

            var moduleService = new ModulesService(assemblyLocation);
            IDevice[] devices = moduleService.GetModules<IDeviceFactory>().SelectMany(d => d.GetDevices()).ToArray();
            var configurators = moduleService.GetModules<IConfigurator>().ToArray();
            foreach (IConfigurator configurator in configurators)
                configurator.Devices = devices;

            var optionsService = new OptionsService(Path.Combine(settingsLocation, "options.store"));
            optionsService.InitOptions(configurators);
            optionsService.InitEntity(this);

            var globalServices = new GlobalServices(
                    optionsService,
                    moduleService,
                    new TimerService()
                );

            _globalContext = new GlobalContext(_coreOptions, globalServices, () => devices, () => _layouts, () => _transitions, () => _deviceHooks);

            _deviceManagers = InitDeviceManagers(devices);
            _layouts = InitLayouts();
            _transitions = InitTransitions();
            _deviceHooks = InitDeviceHooks();
        }

        private ConcurrentDictionary<Identifier, DeviceManager> InitDeviceManagers(IDevice[] devices)
        {
            var result = new ConcurrentDictionary<Identifier, DeviceManager>();

            foreach (var device in devices)
                result.TryAdd(device.Id, new DeviceManager(_globalContext, device));

            return result;
        }

        private ConcurrentDictionary<Identifier, ILayout> InitLayouts()
        {
            var result = new ConcurrentDictionary<Identifier, ILayout>();

            foreach (var initInfo in _coreOptions.LayoutLoadOptions.InitializationInfos)
            {
                var layout = _globalContext.CreateLayout(initInfo);
                if (layout != null)
                    result.TryAdd(initInfo.ChildId, layout);
            }

            return result;
        }

        private ConcurrentDictionary<Identifier, ITransition> InitTransitions()
        {
            var result = new ConcurrentDictionary<Identifier, ITransition>();

            Parallel.ForEach(_coreOptions.TransitionLoadOptions.InitializationInfos, (initInfo) => 
            {
                var transition = _globalContext.CreateTransition(initInfo, t=>t.PerformTransition += TransitionOnPerformTransition);
                if (transition != null)
                    result.TryAdd(initInfo.ChildId, transition);
            });

            return result;
        }

        private ConcurrentDictionary<Identifier, IDeviceHook> InitDeviceHooks()
        {
            var result = new ConcurrentDictionary<Identifier, IDeviceHook>();

            foreach (var deviceHook in _globalContext.Services.ModulesService.GetModules<IDeviceHook>())
            {
                _globalContext.InitializeEntity(deviceHook);
                result[deviceHook.Id] = deviceHook;
            }

            return result;
        }

        private void TransitionOnPerformTransition(object sender, TransitionEventArgs e)
        {
            var deviceManagers = GetDeviceManagers(e.DeviceId);

            if (e.Back)
            {
                foreach (var deviceManager in deviceManagers)
                    deviceManager.SetPreviousLayout(e.LayoutId);
            }
            else
            {
                var layout = _layouts[e.LayoutId];

                foreach (var deviceManager in deviceManagers)
                    deviceManager.SetLayout(layout);
            }
        }

        private IEnumerable<DeviceManager> GetDeviceManagers(Identifier identifier)
        {
            if (_deviceManagers.TryGetValue(identifier, out var manager))
                yield return manager;
        }

        public void Dispose()
        {
            _globalContext.Services.OptionsService.SaveOptions();

            foreach (var deviceManager in _deviceManagers.Values)
                deviceManager.Dispose();

            _deviceManagers.Clear();
        }

        public void Pause()
        {
            foreach (var deviceManager in _deviceManagers.Values)
                deviceManager.Pause();
        }

        public void Resume()
        {
            foreach (var deviceManager in _deviceManagers.Values)
                deviceManager.Resume();
        }
    }
}