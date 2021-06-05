using System;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Drawable;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.AudioSessions
{
    public partial class AudioSessionsLayout
    {
        class AudioDeviceElement : ElementBase
        {
            public override DeviceSize ButtonCount => new DeviceSize(1, 1);

            private readonly AudioSessionsLayout _audioSelectLayout;
            private readonly MediaDeviceInfo _device;

            public AudioDeviceElement(AudioSessionsLayout audioSelectLayout, MediaDeviceInfo device) : base(new Identifier($"ButtonValue.{device.Id}"))
            {
                _audioSelectLayout = audioSelectLayout;
                _device = device;
            }

            protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
            {
                DrawInvoke(new[] {new LayoutDrawElement(new Location(0, 0), DrawKey())});
            }

            private BitmapEx DrawKey()
            {
                var bitmap = LayoutContext.CreateBitmap();
                var fontFamily = GlobalContext.Options.Theme.FontFamily;
                var combName = $"{_device.RealName.Split(' ')[0]}\n{_device.FriendlyName.Split(' ')[0]}";
                DefaultDrawingAlgs.DrawText(bitmap, fontFamily, combName, GlobalContext.Options.Theme.ForegroundColor);

                if (_device.Mute)
                    DefaultDrawingAlgs.SelectElement(bitmap, GlobalContext.Options.Theme);

                return bitmap;
            }

            public override void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
            {
                if (buttonEvent == ButtonEvent.Down && location.X == 0 && location.Y == 0)
                {
                    _audioSelectLayout._mediaDeviceService.SetMute(!_device.Mute, _device);
                    _audioSelectLayout.SetSessionMuted(layoutContext);
                }
            }
        }
    }
}