using System;
using System.Collections.Generic;
using System.Linq;
using Vkm.Api.Basic;
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
        public class AudioSessionsMacroElement: ElementBase
        {
            public override DeviceSize ButtonCount => new DeviceSize(1, 1);

            private readonly AudioSessionsLayout _audioSelectLayout;
            private readonly int _macroId;

            private readonly MacroData _macroData;
            
            public AudioSessionsMacroElement(AudioSessionsLayout audioSelectLayout, int macroId) : base(new Identifier($"ButtonValue.Macro.{macroId}"))
            {
                _audioSelectLayout = audioSelectLayout;
                _macroId = macroId;

                if (!_audioSelectLayout._macroData.TryGetValue(macroId, out _macroData))
                {
                    _macroData = new MacroData();
                    _audioSelectLayout._macroData[macroId] = _macroData;
                }
            }

            protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
            {
                DrawInvoke(new[] {new LayoutDrawElement(new Location(0, 0), DrawKey())});
            }

            private BitmapEx DrawKey()
            {
                var bitmap = LayoutContext.CreateBitmap();
                var fontFamily = GlobalContext.Options.Theme.FontFamily;
                DefaultDrawingAlgs.DrawText(bitmap, fontFamily, _macroId.ToString(), GlobalContext.Options.Theme.ForegroundColor);
                return bitmap;
            }

            public override void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
            {
                if (location.X == 0 && location.Y == 0)
                {
                    if (buttonEvent == ButtonEvent.Up)
                    {
                        foreach (var pair in _macroData.Devices.ToArray())
                        {
                            _audioSelectLayout._mediaDeviceService.SetMute(pair.Value, pair.Key);
                        }
                        foreach (var pair in _macroData.Sessions.ToArray())
                        {
                            _audioSelectLayout._mediaDeviceService.SetMuteSession(pair.Value, pair.Key);
                        }
                        
                        _audioSelectLayout.SetSessionMuted(layoutContext);
                    } 
                    else if (buttonEvent == ButtonEvent.LongPress)
                    {
                        _macroData.Devices.Clear();
                        _macroData.Sessions.Clear();
                        var sessions = _audioSelectLayout._mediaDeviceService.GetSessions();
                        foreach (var session in sessions)
                        {
                            _macroData.Sessions[session] = session.Mute;
                        }
                        var devices = _audioSelectLayout._mediaDeviceService.GetDevices(false);
                        foreach (var device in devices)
                        {
                            _macroData.Devices[device] = device.Mute;
                        }
                    }
                }
            }
        }

        class MacroData
        {
            public readonly Dictionary<MediaDeviceInfo, bool> Devices;
            public readonly Dictionary<MediaSessionInfo, bool> Sessions;

            public MacroData()
            {
                Devices = new Dictionary<MediaDeviceInfo, Boolean>();
                Sessions = new Dictionary<MediaSessionInfo, Boolean>();
            }
        }
    }
}