using System;
using System.Diagnostics;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Common;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.AudioSessions
{
    public partial class AudioSessionsLayout
    {
        class AudioSessionsElement : ElementBase
        {
            public override DeviceSize ButtonCount => new DeviceSize(1, 1);

            private readonly AudioSessionsLayout _audioSelectLayout;
            private readonly MediaSessionInfo _session;

            public AudioSessionsElement(AudioSessionsLayout audioSelectLayout, MediaSessionInfo session) : base(new Identifier($"ButtonValue.{session.SessionIdentifier}"))
            {
                _audioSelectLayout = audioSelectLayout;
                _session = session;
            }

            protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
            {
                DrawInvoke(new[] {new LayoutDrawElement(new Location(0, 0), DrawKey())});
            }

            private BitmapEx DrawKey()
            {
                var bitmap = LayoutContext.CreateBitmap();

                var path = _session.SessionIdentifier.Split('|')[1];
                path = path.Substring(0, path.Length - 40);
                path = DevicePathMapper.FromDevicePath(path);

                var process = Process.GetProcesses().FirstOrDefault(p => p.Id == _session.ProcessId);

                if (path == null && process != null)
                {
                    try
                    {
                        path = process.MainModule.FileName;
                    }
                    catch
                    {
                    }
                }

                if (path != null)
                {
                    var iconRepresentation = _audioSelectLayout._bitmapDownloadService.GetBitmapForExecutable(path).GetAwaiter().GetResult();
                    using (var iconBmpEx = iconRepresentation.CreateBitmap())
                    {
                        BitmapHelpers.ResizeBitmap(iconBmpEx, bitmap);
                    }
                }

                if (_session.Mute)
                    DefaultDrawingAlgs.SelectElement(bitmap, GlobalContext.Options.Theme);

                return bitmap;
            }

            public override void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
            {
                if (buttonEvent == ButtonEvent.Down && location.X == 0 && location.Y == 0)
                {
                    _audioSelectLayout._mediaDeviceService.SetMuteSession(!_session.Mute, _session);
                    _audioSelectLayout.SetSessionMuted(layoutContext);
                }
            }
        }
    }
}