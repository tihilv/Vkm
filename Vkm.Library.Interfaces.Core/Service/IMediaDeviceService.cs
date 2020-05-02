using System;
using Vkm.Api.Service;

namespace Vkm.Library.Interfaces.Service
{
    public interface IMediaDeviceService: IService
    {
        event EventHandler<EventArgs> VolumeChanged;

        void IncreaseVolume();
        void DecreaseVolume();

        bool IsMuted { get; }
        void SetMute(bool value, MediaDeviceInfo? device);
        
        bool HasDevice { get; }

        double Volume { get; }

        MediaDeviceInfo GetDefaultDevice();
        MediaDeviceInfo[] GetDevices(bool? outputDevices);
        void SetDefault(MediaDeviceInfo device);
        Single GetPeakVolumeValue();
        MediaSessionInfo[] GetSessions();
        void SetMuteSession(bool value, MediaSessionInfo session);
    }

    public struct MediaDeviceInfo
    {
        public readonly string Id;
        public readonly string RealName;
        public readonly string FriendlyName;
        public readonly MediaDeviceType Type;
        public readonly bool Mute;

        public MediaDeviceInfo(String id, String realName, String friendlyName, MediaDeviceType type, bool mute)
        {
            Id = id;
            RealName = realName;
            FriendlyName = friendlyName;
            Type = type;
            Mute = mute;
        }
    }

    public struct MediaSessionInfo
    {
        public readonly string SessionIdentifier;
        public readonly uint ProcessId;
        public readonly bool Mute;

        public MediaSessionInfo(String sessionIdentifier, UInt32 processId, Boolean mute)
        {
            SessionIdentifier = sessionIdentifier;
            ProcessId = processId;
            Mute = mute;
        }
    }

    public enum MediaDeviceType: byte
    {
        Unknown,
        Speakers,
        Phone,
        Digital,
        Monitor
    }

}
