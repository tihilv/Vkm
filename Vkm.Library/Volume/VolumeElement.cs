using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Timers;
using CoreAudioApi;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Library.Volume
{
    internal class VolumeElement: ElementBase
    {
        public override DeviceSize ButtonCount => new DeviceSize(1, 2);

        private MMDeviceEnumerator _mmDeviceEnumerator;
        private MMDevice _mmDevice;

        private readonly Timer _buttonPressedTimer;
        private bool _increase;
        
        public VolumeElement(Identifier identifier) : base(identifier)
        {
            _buttonPressedTimer = new Timer();
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
                _mmDevice.AudioEndpointVolume.VolumeStepUp();
            else
                _mmDevice.AudioEndpointVolume.VolumeStepDown();
        }

        public override void EnterLayout(LayoutContext layoutContext)
        {
            base.EnterLayout(layoutContext);

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
            
            _mmDevice.Dispose();
            _mmDeviceEnumerator.Dispose();
        }
        
        void Draw()
        {
            List<LayoutDrawElement> drawElements = new List<LayoutDrawElement>();
            using (var bitmap = DrawLevel())
            {
                for (byte x = 0; x < ButtonCount.Width; x++)
                for (byte y = 0; y < ButtonCount.Height; y++)
                {
                    var part = LayoutContext.CreateBitmap();

                    using (Graphics grD = Graphics.FromImage(part))
                    {
                        grD.DrawImage(bitmap, new Rectangle(0, 0, LayoutContext.IconSize.Width, LayoutContext.IconSize.Height), new Rectangle(x*LayoutContext.IconSize.Width, y*LayoutContext.IconSize.Height, LayoutContext.IconSize.Width, LayoutContext.IconSize.Height), GraphicsUnit.Pixel);
                    }

                    drawElements.Add(new LayoutDrawElement(new Location(x, y), part));
                }
            }

            DrawElementInvoke(drawElements);
        }

        private Bitmap DrawLevel()
        {
            //LayoutContext.IconSize

            var bitmap = new Bitmap(LayoutContext.IconSize.Width * ButtonCount.Width, LayoutContext.IconSize.Height * ButtonCount.Height);
            bitmap.MakeTransparent();

            var baseColor = (_mmDevice.AudioEndpointVolume.Mute) ? GlobalContext.Options.Theme.WarningColor : GlobalContext.Options.Theme.LevelColor;

            var delta = 50;
            Color color1 = Color.FromArgb(baseColor.A, Math.Min(byte.MaxValue, baseColor.R + delta), Math.Min(byte.MaxValue, baseColor.G + delta), Math.Min(byte.MaxValue, baseColor.B + delta));
            Color color2 = Color.FromArgb(baseColor.A, Math.Max(0, baseColor.R - delta), Math.Max(0, baseColor.G - delta), Math.Max(0, baseColor.B - delta));

            using (var graphics = Graphics.FromImage(bitmap))
            using (var brush = new LinearGradientBrush(new Point(0,0), new Point(0, bitmap.Height), color1, color2))
            {
                var top =  (int)(bitmap.Height*(1-_mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar));
                var left =  (int)(bitmap.Width*(1-_mmDevice.AudioEndpointVolume.MasterVolumeLevelScalar));
                graphics.FillPolygon(brush, new Point[] {new Point(bitmap.Width, bitmap.Height), new Point(bitmap.Width, top), new Point(left, top)} );
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
