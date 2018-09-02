using System.Drawing;
using System.Linq;
using CoreAudioApi;
using CoreAudioApi.enumerations;
using CoreAudioApi.ExtendedConfig;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Library.Common;
using Vkm.Library.Volume;

namespace Vkm.Library.AudioSelect
{
    class AudioSelectLayout: LayoutBase
    {
        private string _defaultDeviceId;

        private AudioSelectOptions _options;

        public AudioSelectLayout(Identifier identifier, AudioSelectOptions options) : base(identifier)
        {
            _options = options;
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            DrawElements();
        }

        public override void LeaveLayout()
        {
            base.LeaveLayout();

            ClearElements();
        }

        void DrawElements()
        {
            int width = LayoutContext.ButtonCount.Width - 1;
            AddElement(new Location(width,LayoutContext.ButtonCount.Height-1), GlobalContext.InitializeEntity(new BackElement()));
            AddElement(new Location(width, 0), GlobalContext.InitializeEntity(new VolumeElement(new Identifier(Id.Value + ".Volume"))));

            using (var mmDeviceEnumerator = new MMDeviceEnumerator())
            {
                _defaultDeviceId = mmDeviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia).Id;

                var devices = mmDeviceEnumerator.EnumerateAudioEndPoints(EDataFlow.eRender, DeviceState.DEVICE_STATE_ACTIVE);
                for (int i = 0; i < devices.Count; i++)
                {
                    MMDevice device = devices[i];
                    AddElement(new Location(i % width, i / width), GlobalContext.InitializeEntity(new AudioDeviceElement(this, device)));
                }
            }
        }
        
        void ClearElements()
        {
            foreach (ElementPlacement placement in Elements.ToArray())
                RemoveElement(placement.Element);
        }


        private void SetDefaultDevice(string deviceId)
        {
            _defaultDeviceId = deviceId;

            ClearElements();
            DrawElements();
        }

        class AudioDeviceElement : ElementBase
        {
            public override DeviceSize ButtonCount => new DeviceSize(1, 1);

            private readonly AudioSelectLayout _audioSelectLayout;

            private readonly string _deviceId;
            private readonly string _realName;
            private readonly string _friendlyName;
            private readonly DeviceIcon _icon;

            public AudioDeviceElement(AudioSelectLayout audioSelectLayout, MMDevice device) : base(new Identifier($"ButtonValue.{device.Id}"))
            {
                _audioSelectLayout = audioSelectLayout;
                _deviceId = device.Id;
                _realName = device.RealName;
                _friendlyName = device.FriendlyName;
                _icon = device.Icon;
            }

            public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
            {
                base.EnterLayout(layoutContext, previousLayout);

                DrawInvoke(new [] {new LayoutDrawElement(new Location(0, 0), DrawKey())});
            }

            private BitmapEx DrawKey()
            {
                var bitmap = LayoutContext.CreateBitmap();

                var fontFamily = GlobalContext.Options.Theme.FontFamily;

                string icon = null;

                if (!_audioSelectLayout._options.Names.TryGetValue(_deviceId, out var combName))
                {
                    combName = $"{_realName.Split(' ')[0]}\n{_friendlyName.Split(' ')[0]}";
                }
                else
                {
                    switch (_icon)
                    {
                        case DeviceIcon.Speakers:
                            icon = FontAwesomeRes.fa_volume_down;
                            break;
                        case DeviceIcon.Phone:
                            icon = FontAwesomeRes.fa_phone;
                            break;
                        case DeviceIcon.Digital:
                            icon = FontAwesomeRes.fa_usb;
                            break;
                        case DeviceIcon.Monitor:
                            icon = FontAwesomeRes.fa_tv;
                            break;
                    }
                }

                if (icon != null)
                    DefaultDrawingAlgs.DrawCaptionedIcon(bitmap, FontService.Instance.AwesomeFontFamily, icon, fontFamily, combName, combName, GlobalContext.Options.Theme.ForegroundColor);
                else
                    DefaultDrawingAlgs.DrawText(bitmap, fontFamily, combName, GlobalContext.Options.Theme.ForegroundColor);

                if (_deviceId == _audioSelectLayout._defaultDeviceId)
                {
                    using (var graphics = bitmap.CreateGraphics())
                    using (var pen = new Pen(GlobalContext.Options.Theme.ForegroundColor, 3))
                    {
                        graphics.DrawRectangle(pen, 0, 0, bitmap.Width, bitmap.Height);
                    }
                }

                return bitmap;
            }

            public override bool ButtonPressed(Location location, bool isDown)
            {
                if (isDown && location.X == 0 && location.Y == 0)
                {
                    PolicyConfig.SetDefaultEndpoint(_deviceId, ERole.eMultimedia);
                    _audioSelectLayout.SetDefaultDevice(_deviceId);
                    return true;
                }

                return false;
            }
        }
    }
}
