using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vkm.Api.Data;
using Vkm.Api.Device;
using Vkm.Api.Identification;
using Vkm.Api.Service;
using Vkm.Api.Transition;

namespace Vkm.Kernel
{
    public partial class VkmKernel: IDisposable
    {
        private readonly GlobalContext _globalContext;

        private readonly ConcurrentDictionary<Identifier, DeviceManager> _deviceManagers;

        private readonly ISelfHostedService[] _selfHostedServices;

        public VkmKernel()
        {
            var assemblyLocation = Path.GetDirectoryName(typeof(VkmKernel).Assembly.Location);
            var settingsLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vkm");

            var moduleService = new ModulesService(assemblyLocation);

            var optionsService = new OptionsService(Path.Combine(settingsLocation, "options.store"));

            var globalServices = new GlobalServices(
                    optionsService,
                    moduleService,
                    new TimerService()
                );

            _globalContext = new GlobalContext(globalServices);

            _deviceManagers = InitDeviceManagers(_globalContext.Devices);
            
            _selfHostedServices = _globalContext.GetServices<ISelfHostedService>().ToArray();
            
            StartTransitions();
        }

        private void StartTransitions()
        {
            foreach (var transition in _globalContext.Transitions.Values)
            {
                transition.PerformTransition += TransitionOnPerformTransition;
                transition.Run();
            }
        }

        private ConcurrentDictionary<Identifier, DeviceManager> InitDeviceManagers(IDevice[] devices)
        {
            var result = new ConcurrentDictionary<Identifier, DeviceManager>();

            foreach (var device in devices)
                result.TryAdd(device.Id, new DeviceManager(_globalContext, device));

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
                var layout = _globalContext.Layouts[e.LayoutId];

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
