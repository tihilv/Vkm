using System;

namespace Vkm.Api.Basic
{
    [Serializable]
    public struct IconSize
    {
        public readonly ushort Width;
        public readonly ushort Height;


        public IconSize(ushort width, ushort height)
        {
            Width = width;
            Height = height;
        }
    }
}