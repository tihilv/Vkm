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

        bool HasDevice { get; }

        double Volume { get; }

        MediaDeviceInfo GetDefaultDevice();
        MediaDeviceInfo[] GetDevices();
        void SetDefault(MediaDeviceInfo device);
    }

    public struct MediaDeviceInfo
    {
        public readonly string Id;
        public readonly string RealName;
        public readonly string FriendlyName;
        public readonly MediaDeviceType Type;

        public MediaDeviceInfo(string id, string realName, string friendlyName, MediaDeviceType type)
        {
            Id = id;
            RealName = realName;
            FriendlyName = friendlyName;
            Type = type;
        }
    }

    public enum MediaDeviceType
    {
        Unknown,
        Speakers,
        Phone,
        Digital,
        Monitor
    }

}
