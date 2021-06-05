using System;
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
using Vkm.Api.Transition;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.Volume
{
    internal class VolumeElement: ElementBase
    {
        public override DeviceSize ButtonCount => new DeviceSize(1, 2);

        private IMediaDeviceService _mediaDeviceService;

        private readonly System.Timers.Timer _buttonPressedTimer;
        private bool _increase;

        private int _pressedCount;

        private Tuple<bool, double, float> _lastValues;

        public VolumeElement(Identifier identifier) : base(identifier)
        {
            _buttonPressedTimer = new System.Timers.Timer();
            _buttonPressedTimer.Elapsed += ButtonPressedTimerOnElapsed;
        }

        public override void Init()
        {
            base.Init();

            RegisterTimer(new TimeSpan(0,0,1), () => Draw());

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

        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            _mediaDeviceService.VolumeChanged += AudioEndpointVolume_OnVolumeNotification;

            Draw();
        }

        private void AudioEndpointVolume_OnVolumeNotification(object sender, EventArgs args)
        {
            Draw();
        }

        protected override void OnLeavingLayout()
        {
            _mediaDeviceService.VolumeChanged -= AudioEndpointVolume_OnVolumeNotification;
        }

        protected override void OnLeavedLayout()
        {
            _lastValues = null;
        }
        
        void Draw()
        {
            var muted = _mediaDeviceService.IsMuted;
            var volume = _mediaDeviceService.Volume;
            var peakValue = _mediaDeviceService.GetPeakVolumeValue();

            var newValues = new Tuple<bool, double, float>(muted, volume, peakValue);
            if (!newValues.Equals(_lastValues))
            {
                _lastValues = newValues;

                using (var bitmap = DrawLevel(muted, volume, peakValue))
                {
                    DrawInvoke(BitmapHelpers.ExtractLayoutDrawElements(bitmap, ButtonCount, 0, 0, LayoutContext, new TransitionInfo(TransitionType.ElementUpdate, new TimeSpan(0, 0, 0, 0, 500))));
                }
            }
        }

        private BitmapEx DrawLevel(bool muted, double volume, float peakValue)
        {
            var bitmap = new BitmapEx((ushort)(LayoutContext.IconSize.Width * ButtonCount.Width), (ushort)(LayoutContext.IconSize.Height * ButtonCount.Height));
            bitmap.MakeTransparent();

            var delta = 50;
            var volumePeakWidth = bitmap.Width / 3;
            var selectorWidth = bitmap.Width - volumePeakWidth;

            var baseColor = muted ? GlobalContext.Options.Theme.WarningColor : GlobalContext.Options.Theme.LevelColor;

            
            Color color1 = Color.FromArgb(baseColor.A, Math.Min(byte.MaxValue, baseColor.R + delta), Math.Min(byte.MaxValue, baseColor.G + delta), Math.Min(byte.MaxValue, baseColor.B + delta));
            Color color2 = Color.FromArgb(baseColor.A, Math.Max(0, baseColor.R - delta), Math.Max(0, baseColor.G - delta), Math.Max(0, baseColor.B - delta));

            using (var graphics = bitmap.CreateGraphics())
            using (var brush = new LinearGradientBrush(new Point(0,0), new Point(0, bitmap.Height), color1, color2))
            using (var pen = new Pen(baseColor, 2))
            {
                if (_mediaDeviceService.HasDevice)
                {
                    var top = (int) (bitmap.Height * (1 - volume));
                    var left = (int) (selectorWidth * (1 - volume));

                    var pointsCurrent = new [] {new Point(selectorWidth, bitmap.Height), new Point(selectorWidth, top), new Point(left, top)};
                    var pointsFull = new [] {new Point(selectorWidth, bitmap.Height), new Point(selectorWidth, 0), new Point(0, 0)};

                    graphics.FillPolygon(brush, pointsCurrent);
                    graphics.DrawPolygon(pen, pointsFull);

                    var selectorTop = (int) (bitmap.Height * (1 - peakValue));

                    selectorWidth += bitmap.Width / 10;
                    graphics.FillPolygon(brush, new [] {new Point(selectorWidth, bitmap.Height), new Point(bitmap.Width, bitmap.Height), new Point(bitmap.Width, selectorTop), new Point(selectorWidth, selectorTop)});
                }
            }

            return bitmap;
        }

        public override void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
        {
            if (buttonEvent == ButtonEvent.Down || buttonEvent == ButtonEvent.Up)
            {
                _buttonPressedTimer.Interval = 400;
                _buttonPressedTimer.Enabled = buttonEvent == ButtonEvent.Down;
                _increase = location.Y == 0;

                if (buttonEvent == ButtonEvent.Down)
                    _pressedCount++;
                else
                    _pressedCount--;

                if (_pressedCount < 0)
                    _pressedCount = 0;

                if (_pressedCount > 1)
                {
                    if (buttonEvent == ButtonEvent.Down)
                    {
                        _mediaDeviceService.SetMute(!_mediaDeviceService.IsMuted, null);
                    }
                }
                else if (buttonEvent == ButtonEvent.Down)
                    DoVolumeChange();
            }
        }
    }
}
