namespace Vkm.Api.Basic
{
    public struct IconSize
    {
        public readonly int Width;
        public readonly int Height;


        public IconSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static IconSize operator +(IconSize a, IconSize b)
        {
            return new IconSize(a.Width + b.Width, a.Height + b.Height);
        }
    }
}