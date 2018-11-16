using System;
using System.Collections.Generic;
using System.Diagnostics;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Service;
using Vkm.Intercom.Dispatchers;
using Vkm.Intercom.Service.Api;
using Vkm.Intercom.Service.Visual;

namespace Vkm.Intercom.Service
{
    internal static class Constants
    {
        public static readonly string DispatcherName = $"Vkm.Intercom.Session{Process.GetCurrentProcess().SessionId}";
    }

    public class IntercomSelfHostedService : ISelfHostedService, IInitializable, IRemoteService
    {
        private IntercomMasterDispatcher<IVkmRemoteService, IVkmRemoteCallback> _intercomDispatcher;

        private GlobalContext _globalContext;

        static Identifier Identifier = new Identifier("Vkm.IntercomService");

        public Identifier Id => Identifier;

        public string Name => "Intercom Service";

        public void InitContext(GlobalContext context)
        {
            _globalContext = context;
        }

        public void Init()
        {
            _intercomDispatcher = new IntercomMasterDispatcher<IVkmRemoteService, IVkmRemoteCallback>(Constants.DispatcherName, () => new InteropService(_globalContext));
        }

        class InteropServiceCallback : IVkmRemoteCallback
        {
            private readonly IIntercomClient _callbackClient;

            public InteropServiceCallback(IIntercomClient callbackClient)
            {
                _callbackClient = callbackClient;
            }

            public void ButtonPressed(Identifier layoutId, Location location, ButtonEvent buttonEvent)
            {
                _callbackClient.Execute(nameof(IVkmRemoteCallback.ButtonPressed), layoutId, location, buttonEvent);
            }
        }


        class InteropService : IVkmRemoteService, IRemoteServiceWithCallback, IDisposable
        {
            private readonly GlobalContext _globalContext;

            private readonly Dictionary<Identifier, RemoteLayout> _layouts;

            private InteropServiceCallback _callbackClient;

            public InteropService(GlobalContext globalContext)
            {
                _globalContext = globalContext;

                _layouts = new Dictionary<Identifier, RemoteLayout>();
            }

            public void RegisterChannel(IIntercomClient client)
            {
                _callbackClient = new InteropServiceCallback(client);
            }

            public IntercomDeviceInfo[] GetDevices()
            {
                IntercomDeviceInfo[] result = new IntercomDeviceInfo[_globalContext.Devices.Length];

                for (int i = 0; i < result.Length; i++)
                {
                    var device = _globalContext.Devices[i];

                    result[i] = new IntercomDeviceInfo(device.Id, device.ButtonCount, device.IconSize);
                }

                return result;
            }

            public Identifier GetLayout(string name)
            {
                var identifier = new Identifier($"Vkm.IntercomService.Layout.{name}");

                if (!_globalContext.Layouts.ContainsKey(identifier))
                {
                    var layout = _globalContext.InitializeEntity(new RemoteLayout(identifier, _callbackClient));
                    _layouts[identifier] = layout;
                    _globalContext.Layouts[identifier] = layout;
                }

                return identifier;
            }

            public void RemoveLayout(Identifier layoutId)
            {
                _globalContext.RemoveLayout(layoutId);
            }

            public void SwitchToLayout(Identifier deviceId, Identifier layoutId)
            {
                IntercomTransitionFactory.Transition.SwitchToLayout(deviceId, layoutId);
            }

            public void SetBitmap(Identifier layoutId, Location location, byte[] bitmapBytes)
            {
                _layouts[layoutId].SetBitmap(location, bitmapBytes);
            }

            public void RemoveBitmap(Identifier layoutId, Location location)
            {
                _layouts[layoutId].RemoveBitmap(location);
            }

            public void Dispose()
            {
                foreach (var layoutId in _layouts.Keys)
                    _globalContext.RemoveLayout(layoutId);

                _layouts.Clear();
            }
        }
    }
}