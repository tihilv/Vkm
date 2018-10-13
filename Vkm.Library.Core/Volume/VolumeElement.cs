using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Timers;
using Vkm.Api.Basic;
using Vkm.Api.Common;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.Volume
{
    internal class VolumeElement: ElementBase
    {
        public override DeviceSize ButtonCount => new DeviceSize(1, 2);

        private IMediaDeviceService _mediaDeviceService;

        private readonly System.Timers.Timer _buttonPressedTimer;
        private bool _increase;
        
        public VolumeElement(Identifier identifier) : base(identifier)
        {
            _buttonPressedTimer = new System.Timers.Timer();
            _buttonPressedTimer.Elapsed += ButtonPressedTimerOnElapsed;
        }

        public override void Init()
        {
            base.Init();

            _mediaDeviceService = GlobalContext.GetServices<IMediaDeviceService>().First();
        }

        private void ButtonPressedTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            DoVolumeChange();

            _buttonPressedTimer.Interval *= 2.0/3.0;
        }

        private void DoVolumeChange()
        {
            if (_increase)
                _mediaDeviceService.IncreaseVolume();
            else
                _mediaDeviceService.DecreaseVolume();
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            _mediaDeviceService.VolumeChanged += AudioEndpointVolume_OnVolumeNotification;

            Draw();
        }

        private void AudioEndpointVolume_OnVolumeNotification(object sender, EventArgs args)
        {
            Draw();
        }

        public override void LeaveLayout()
        {
            _mediaDeviceService.VolumeChanged -= AudioEndpointVolume_OnVolumeNotification;
            
            base.LeaveLayout();
        }
        
        void Draw()
        {
            List<LayoutDrawElement> drawElements = new List<LayoutDrawElement>();
            using (var bitmap = DrawLevel())
            {
                DrawInvoke(BitmapHelpers.ExtractLayoutDrawElements(bitmap, ButtonCount, 0, 0, LayoutContext));
            }

        }

        private BitmapEx DrawLevel()
        {
            var bitmap = new BitmapEx(LayoutContext.IconSize.Width * ButtonCount.Width, LayoutContext.IconSize.Height * ButtonCount.Height);
            bitmap.MakeTransparent();

            var baseColor = (_mediaDeviceService.IsMuted) ? GlobalContext.Options.Theme.WarningColor : GlobalContext.Options.Theme.LevelColor;

            var delta = 50;
            Color color1 = Color.FromArgb(baseColor.A, Math.Min(byte.MaxValue, baseColor.R + delta), Math.Min(byte.MaxValue, baseColor.G + delta), Math.Min(byte.MaxValue, baseColor.B + delta));
            Color color2 = Color.FromArgb(baseColor.A, Math.Max(0, baseColor.R - delta), Math.Max(0, baseColor.G - delta), Math.Max(0, baseColor.B - delta));

            using (var graphics = bitmap.CreateGraphics())
            using (var brush = new LinearGradientBrush(new Point(0,0), new Point(0, bitmap.Height), color1, color2))
            using (var pen = new Pen(baseColor, 2))
            {
                if (_mediaDeviceService.HasDevice)
                {
                    var top = (int) (bitmap.Height * (1 - _mediaDeviceService.Volume));
                    var left = (int) (bitmap.Width * (1 - _mediaDeviceService.Volume));

                    var pointsCurrent = new [] {new Point(bitmap.Width, bitmap.Height), new Point(bitmap.Width, top), new Point(left, top)};
                    var pointsFull = new [] {new Point(bitmap.Width, bitmap.Height), new Point(bitmap.Width, 0), new Point(0, 0)};

                    graphics.FillPolygon(brush, pointsCurrent);
                    graphics.DrawPolygon(pen, pointsFull);
                }
            }

            return bitmap;
        }

        public override bool ButtonPressed(Location location, bool isDown)
        {
            _buttonPressedTimer.Interval = 400;
            _buttonPressedTimer.Enabled = isDown;
            _increase = location.Y == 0;

            if (isDown)
                DoVolumeChange();

            return true;
        }
    }
}
