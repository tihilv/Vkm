using System;

namespace Vkm.Api.Layout
{
    public class DrawEventArgs : EventArgs
    {
        public LayoutDrawElement[] Elements { get; private set; }

        public DrawEventArgs(LayoutDrawElement[] elements)
        {
            Elements = elements;
        }
    }
}