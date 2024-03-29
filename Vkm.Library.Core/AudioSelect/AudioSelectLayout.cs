﻿using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Drawable;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;
using Vkm.Library.Common;
using Vkm.Library.Interfaces.Service;
using Vkm.Library.Volume;

namespace Vkm.Library.AudioSelect
{
    class AudioSelectLayout: LayoutBase
    {
        private string _defaultDeviceId;

        private AudioSelectOptions _options;

        private IMediaDeviceService _mediaDeviceService;

        public AudioSelectLayout(Identifier identifier, AudioSelectOptions options) : base(identifier)
        {
            _options = options;
        }

        public override void Init()
        {
            base.Init();

            _mediaDeviceService = GlobalContext.GetServices<IMediaDeviceService>().First();
        }

        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            DrawElements(layoutContext);
        }

        protected override void OnLeavedLayout()
        {
            ClearElements();
        }

        void DrawElements(LayoutContext layoutContext)
        {
            int width = layoutContext.ButtonCount.Width - 1;
            AddElement(new Location(width,layoutContext.ButtonCount.Height-1), GlobalContext.InitializeEntity(new BackElement()));
            AddElement(new Location(width, 0), GlobalContext.InitializeEntity(new VolumeElement(new Identifier(Id.Value + ".Volume"))));

            _defaultDeviceId = _mediaDeviceService.GetDefaultDevice().Id;
            var devices = _mediaDeviceService.GetDevices(true);

            var elements = devices.Select(device => GlobalContext.InitializeEntity(new AudioDeviceElement(this, device)));
            AddElementsInRectangle(elements, 0,0,(byte)(layoutContext.ButtonCount.Width - 2),(byte)(layoutContext.ButtonCount.Height - 1));
        }
        
        private void SetDefaultDevice(string deviceId, LayoutContext layoutContext)
        {
            _defaultDeviceId = deviceId;

            ClearElements();
            DrawElements(layoutContext);
        }

        void ClearElements()
        {
            foreach (ElementPlacement placement in Elements.ToArray())
                RemoveElement(placement.Element);
        }


        class AudioDeviceElement : ElementBase
        {
            public override DeviceSize ButtonCount => new DeviceSize(1, 1);

            private readonly AudioSelectLayout _audioSelectLayout;

            private readonly MediaDeviceInfo _device;

            public AudioDeviceElement(AudioSelectLayout audioSelectLayout, MediaDeviceInfo device) : base(new Identifier($"ButtonValue.{device.Id}"))
            {
                _audioSelectLayout = audioSelectLayout;
                _device = device;
            }

            protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
            {
                DrawInvoke(new [] {new LayoutDrawElement(new Location(0, 0), DrawKey())});
            }

            private BitmapEx DrawKey()
            {
                var bitmap = LayoutContext.CreateBitmap();

                var fontFamily = GlobalContext.Options.Theme.FontFamily;

                string icon = null;

                if (!_audioSelectLayout._options.Names.TryGetValue(_device.Id, out var combName))
                {
                    combName = $"{_device.RealName.Split(' ')[0]}\n{_device.FriendlyName.Split(' ')[0]}";
                }
                else
                {
                    switch (_device.Type)
                    {
                        case MediaDeviceType.Speakers:
                            icon = FontAwesomeRes.fa_volume_down;
                            break;
                        case MediaDeviceType.Phone:
                            icon = FontAwesomeRes.fa_phone;
                            break;
                        case MediaDeviceType.Digital:
                            icon = FontAwesomeRes.fa_usb;
                            break;
                        case MediaDeviceType.Monitor:
                            icon = FontAwesomeRes.fa_tv;
                            break;
                    }
                }

                if (icon != null)
                    DefaultDrawingAlgs.DrawCaptionedIcon(bitmap, FontService.Instance.AwesomeFontFamily, icon, fontFamily, combName, combName, GlobalContext.Options.Theme.ForegroundColor);
                else
                    DefaultDrawingAlgs.DrawText(bitmap, fontFamily, combName, GlobalContext.Options.Theme.ForegroundColor);

                if (_device.Id == _audioSelectLayout._defaultDeviceId)
                    DefaultDrawingAlgs.SelectElement(bitmap, GlobalContext.Options.Theme);

                return bitmap;
            }

            public override void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
            {
                if (buttonEvent == ButtonEvent.Down && location.X == 0 && location.Y == 0)
                {
                    _audioSelectLayout._mediaDeviceService.SetDefault(_device);
                    _audioSelectLayout.SetDefaultDevice(_device.Id, layoutContext);
                }
            }
        }
    }
}
