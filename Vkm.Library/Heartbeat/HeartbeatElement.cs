using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;
using Vkm.Common.Win32.Win32;

namespace Vkm.Library.Heartbeat
{
    class HeartbeatElement: ElementBase
    {
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _ramCounter;
        private long _totalMemory;

        private readonly Queue<int> _cpuLoad;
        private readonly Queue<int> _memoryPercentage;

        private long _prevSent;
        private long _prevRecv;

        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public HeartbeatElement(Identifier identifier) : base(identifier)
        {
            _cpuLoad = new Queue<int>();
            _memoryPercentage = new Queue<int>();
        }

        public override void Init()
        {
            base.Init();

            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            Win32.GetPhysicallyInstalledSystemMemory(out _totalMemory);
            _totalMemory /= 1024;

            RegisterTimer(new TimeSpan(0,0,0,1), Tick);
        }

        private void Tick()
        {
            _cpuLoad.Enqueue((int) _cpuCounter.NextValue());
            _memoryPercentage.Enqueue((int) (100 - _ramCounter.NextValue() * 100 / _totalMemory));

            var networkStats = NetworkInterface.GetAllNetworkInterfaces().Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback).Select(n => n.GetIPv4Statistics()).ToArray();

            var sent = networkStats.Sum(n => n.BytesSent);
            var recv = networkStats.Sum(n => n.BytesReceived);

            double mbpsSentSpeed = 8 * (sent - _prevSent) / (1024.0 * 1024);
            double mbpsReceivedSpeed = 8 * (recv - _prevRecv) / (1024.0 * 1024);


            var bitmap = LayoutContext.CreateBitmap();
            if (_prevSent != 0 || _prevRecv != 0)
                DefaultDrawingAlgs.DrawTexts(bitmap, GlobalContext.Options.Theme.FontFamily, $"{mbpsReceivedSpeed:F2}\n{mbpsSentSpeed:F2}", "", "888888", GlobalContext.Options.Theme.ForegroundColor);

            _prevSent = sent;
            _prevRecv = recv;

            DrawLine(_cpuLoad, bitmap, GlobalContext.Options.Theme.ForegroundColor);
            DrawLine(_memoryPercentage, bitmap, Color.Aquamarine);
            
            DrawInvoke(new[] {new LayoutDrawElement(new Location(0, 0), bitmap)});
        }

        private void DrawLine(Queue<int> queue, BitmapEx bitmap, Color color)
        {
            while (queue.Count > bitmap.Width)
                queue.Dequeue();

            if (queue.Count > 1)
            {

                var array = queue.ToArray();

                Point[] points = new Point[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    points[i] = new Point(i, bitmap.Height - bitmap.Height * array[i] / 100);
                }

                using (var graphics = bitmap.CreateGraphics())
                using (var pen = new Pen(color))
                using (var brush = new SolidBrush(color))
                {
                    graphics.DrawCurve(pen, points);
                }

                
            }
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            Tick();
        }
    }
}
