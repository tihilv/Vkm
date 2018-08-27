using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Timers;
using CoreAudioApi;
using Microsoft.Office.Interop.Outlook;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Library.Common;

namespace Vkm.Library.Volume
{
    internal class VolumeElement: ElementBase
    {
        public override DeviceSize ButtonCount => new DeviceSize(1, 2);

        private MMDeviceEnumerator _mmDeviceEnumerator;
        private MMDevice _mmDevice;

        private readonly System.Timers.Timer _buttonPressedTimer;
        private bool _increase;
        
        public VolumeElement(Identifier identifier) : base(identifier)
        {
            _buttonPressedTimer = new System.Timers.Timer();
            _buttonPressedTimer.Elapsed += ButtonPressedTimerOnElapsed;
        }

        private void ButtonPressedTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            DoVolumeChange();

            _buttonPressedTimer.Interval *= 2.0/3.0;
        }

        private void DoVolumeChange()
        {
            if (_increase)
                _mmDevice?.AudioEndpointVolume.VolumeStepUp();
            else
                _mmDevice?.AudioEndpointVolume.VolumeStepDown();
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            _mmDeviceEnumerator = new MMDeviceEnumerator();
            _mmDevice = _mmDeviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

            _mmDevice.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;

            Draw();
        }

        private void AudioEndpointVolume_OnVolumeNotification(object sender, VolumeNotificationEventArgs e)
        {
            Draw();
        }

        public override void LeaveLayout()
        {
            base.LeaveLayout();
            
            DisposeHelper.DisposeAndNull(ref _mmDevice);
            DisposeHelper.DisposeAndNull(ref _mmDeviceEnumerator);
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

            var baseColor = (_mmDevice?.AudioEndpointVolume.Mute??false) ? GlobalContext.Options.Theme.WarningColor : GlobalContext.Options.Theme.LevelColor;

            var delta = 50;
            Color color1 = Color.FromArgb(baseColor.A, Math.Min(byte.MaxValue, baseColor.R + delta), Math.Min(byte.MaxValue, baseColor.G + delta), Math.Min(byte.MaxValue, baseColor.B + delta));
            Color color2 = Color.FromArgb(baseColor.A, Math.Max(0, baseColor.R - delta), Math.Max(0, baseColor.G - delta), Math.Max(0, baseColor.B - delta));

            using (var graphics = bitmap.CreateGraphics())
            using (var brush = new LinearGradientBrush(new Point(0,0), new Point(0, bitmap.Height), color1, color2))
            {
                if (_mmDevice != null)
                {
                    var top =  (int)(bitmap.Height*(1-_mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar));
                    var left =  (int)(bitmap.Width*(1-_mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar));
                    graphics.FillPolygon(brush, new Point[] {new Point(bitmap.Width, bitmap.Height), new Point(bitmap.Width, top), new Point(left, top)} );
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
