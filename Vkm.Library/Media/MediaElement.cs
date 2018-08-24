using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Vkm.Api;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Library.Media
{
    internal class MediaElement: ElementBase
    {
        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        private IntPtr _osdWindowPtr;

        public MediaElement(Identifier id) : base(id)
        {
            
        }

        public override void Init()
        {
            base.Init();

            _osdWindowPtr = FindOSDWindow();
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);
        }

        public override void LeaveLayout()
        {
            base.LeaveLayout();
        }

        private IntPtr FindOSDWindow()
        {
            IntPtr hwndRet = IntPtr.Zero;
            IntPtr hwndHost = IntPtr.Zero;

            int pairCount = 0;

            // search for all windows with class 'NativeHWNDHost'

            while ((hwndHost = Win32.FindWindowEx(IntPtr.Zero, hwndHost, "NativeHWNDHost", "")) != IntPtr.Zero)
            {
                // if this window has a child with class 'DirectUIHWND' it might be the volume OSD

                if (Win32.FindWindowEx(hwndHost, IntPtr.Zero, "DirectUIHWND", "") != IntPtr.Zero)
                {
                    if (pairCount == 0)
                    {
                        hwndRet = hwndHost;
                    }

                    pairCount++;

                    if (pairCount > 1)
                        return IntPtr.Zero;
                }
            }

            Win32.RECT rect;
            var b = Win32.GetWindowRect(new HandleRef(this, hwndRet), out rect);
            var bitmap = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                b = Win32.PrintWindow(hwndRet, g.GetHdc(), 0);
            }



            return hwndRet;
        }

    }
}
