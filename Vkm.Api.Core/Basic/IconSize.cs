using System;

namespace Vkm.Api.Basic
{
    [Serializable]
    public struct IconSize
    {
        public readonly int Width;
        public readonly int Height;


        public IconSize(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}