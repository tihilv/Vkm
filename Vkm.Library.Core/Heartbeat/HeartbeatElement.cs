﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.Heartbeat
{
    class HeartbeatElement: ElementBase
    {
        private ISystemStatusService _systemStatusService;
        
        private long _prevSent;
        private long _prevRecv;


        private readonly Queue<int> _cpuLoad;
        private readonly Queue<int> _memoryPercentage;
        
        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public HeartbeatElement(Identifier identifier) : base(identifier)
        {
            _cpuLoad = new Queue<int>();
            _memoryPercentage = new Queue<int>();
        }

        public override void Init()
        {
            base.Init();

            _systemStatusService = GlobalContext.GetServices<ISystemStatusService>().First();

            RegisterTimer(new TimeSpan(0,0,0,1), Tick);
        }

        private void Tick()
        {
            _cpuLoad.Enqueue(_systemStatusService.GetCpuLoad());
            _memoryPercentage.Enqueue((int) (100 - _systemStatusService.GetFreeMemoryMBytes() * 100 / _systemStatusService.GetTotalMemoryMBytes()));

            var stats = _systemStatusService.GetNetworkStats();

            var sent = stats.Sent;
            var recv = stats.Received;

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

                DefaultDrawingAlgs.DrawPlot(bitmap, color, array, 0, bitmap.Height, minValue: 0, maxValue: 100);
            }
        }

        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            Tick();
        }
    }
}
