using System;
using System.Collections.Generic;
using CoreAudioApi;
using CoreAudioApi.enumerations;
using CoreAudioApi.ExtendedConfig;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.Service
{
    class MediaDeviceService: IMediaDeviceService, IInitializable
    {
        public Identifier Id => new Identifier("Vkm.MediaDeviceService");
        public string Name => "Media Device Service";

        private MMDeviceEnumerator _mmDeviceEnumerator;
        private MMDevice _mmDevice;

        public event EventHandler<EventArgs> VolumeChanged;

        public void IncreaseVolume()
        {
            _mmDevice?.AudioEndpointVolume.VolumeStepUp();
        }

        public void DecreaseVolume()
        {
            _mmDevice?.AudioEndpointVolume.VolumeStepDown();
        }

        public bool IsMuted
        {
            get
            {
                return _mmDevice?.AudioEndpointVolume.Mute ?? false;
            }
        }

        public bool HasDevice => _mmDevice != null;

        public double Volume => _mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar;

        public MediaDeviceInfo GetDefaultDevice()
        {
            using (var mmDeviceEnumerator = new MMDeviceEnumerator())
            {
                var defaultDevice = mmDeviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                return ParseDevice(defaultDevice);
            }
        }

        public MediaDeviceInfo[] GetDevices()
        {
            List<MediaDeviceInfo> result = new List<MediaDeviceInfo>();

            using (var mmDeviceEnumerator = new MMDeviceEnumerator())
            {
                var devices = mmDeviceEnumerator.EnumerateAudioEndPoints(EDataFlow.eRender, DeviceState.DEVICE_STATE_ACTIVE);
                for (int i = 0; i < devices.Count; i++)
                {
                    MMDevice device = devices[i];
                    
                    result.Add(ParseDevice(device));
                }
            }

            return result.ToArray();
        }

        MediaDeviceInfo ParseDevice(MMDevice device)
        {
            MediaDeviceType type = MediaDeviceType.Unknown;
            switch (device.Icon)
            {
                case DeviceIcon.Speakers:
                    type = MediaDeviceType.Speakers;
                    break;
                case DeviceIcon.Phone:
                    type = MediaDeviceType.Phone;
                    break;
                case DeviceIcon.Digital:
                    type = MediaDeviceType.Digital;
                    break;
                case DeviceIcon.Monitor:
                    type = MediaDeviceType.Monitor;
                    break;
            }

            return new MediaDeviceInfo(device.Id, device.RealName, device.FriendlyName, type);
        }

        public void SetDefault(MediaDeviceInfo device)
        {
            PolicyConfig.SetDefaultEndpoint(device.Id, ERole.eMultimedia);
            SetDevice();
        }

        public float GetPeakVolumeValue()
        {
            return _mmDevice?.AudioMeterInformation.MasterPeakValue??0;
        }

        public void InitContext(GlobalContext context)
        {
            
        }

        public void Init()
        {
            _mmDeviceEnumerator = new MMDeviceEnumerator();
            SetDevice();
        }

        private void AudioEndpointVolume_OnVolumeNotification(object sender, VolumeNotificationEventArgs e)
        {
            VolumeChanged?.Invoke(this, EventArgs.Empty);
        }

        void SetDevice()
        {
            var device = _mmDeviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            if (_mmDevice != null)
            {
                _mmDevice.AudioEndpointVolume.OnVolumeNotification -= AudioEndpointVolume_OnVolumeNotification;
            }

            _mmDevice = device;
            _mmDevice.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;

            VolumeChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
