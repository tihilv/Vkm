using System;

namespace Vkm.Api.Basic
{
    [Serializable]
    public struct DeviceSize
    {
        public readonly byte Width;
        public readonly byte Height;


        public DeviceSize(byte width, byte height)
        {
            Width = width;
            Height = height;
        }

        public static DeviceSize operator +(DeviceSize a, DeviceSize b)
        {
            return new DeviceSize((byte) (a.Width + b.Width), (byte) (a.Height + b.Height));
        }
    }
}