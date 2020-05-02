using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Library.Common;
using Vkm.Library.Interfaces.Service;
using Vkm.Library.Interfaces.Services;

namespace Vkm.Library.AudioSessions
{
    public partial class AudioSessionsLayout: LayoutBase
    {
        private readonly AudioSessionsOptions _options;
        private IMediaDeviceService _mediaDeviceService;
        private IBitmapDownloadService _bitmapDownloadService;

        private Dictionary<int, MacroData> _macroData;

        public AudioSessionsLayout(Identifier identifier, AudioSessionsOptions options) : base(identifier)
        {
            _options = options;
            _macroData = new Dictionary<Int32, MacroData>();
        }
        
        public override void Init()
        {
            base.Init();

            _mediaDeviceService = GlobalContext.GetServices<IMediaDeviceService>().First();
            _bitmapDownloadService = GlobalContext.GetServices<IBitmapDownloadService>().First();
        }
        
        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            DrawElements();
        }

        protected override void OnLeavedLayout()
        {
            ClearElements();
        }
        
        void DrawElements()
        {
            int width = LayoutContext.ButtonCount.Width - 1;
            AddElement(new Location(width,LayoutContext.ButtonCount.Height-1), GlobalContext.InitializeEntity(new BackElement()));

            for (int i = 0; i < LayoutContext.ButtonCount.Height-1; i++)
            {
                AddElement(new Location(width,i), GlobalContext.InitializeEntity(new AudioSessionsMacroElement(this, (i+1))));
            }

            var sessions = _mediaDeviceService.GetSessions();
            var devices = _mediaDeviceService.GetDevices(false);
            
            List<IElement> elements = new List<IElement>(); 
            elements.AddRange(sessions.Select(device => GlobalContext.InitializeEntity(new AudioSessionsElement(this, device))).ToList());
            elements.AddRange(devices.Select(device => GlobalContext.InitializeEntity(new AudioDeviceElement(this, device))));
            
            AddElementsInRectangle(elements, 0,0,(byte)(LayoutContext.ButtonCount.Width - 2),(byte)(LayoutContext.ButtonCount.Height - 1));
        }
        
        void ClearElements()
        {
            foreach (ElementPlacement placement in Elements.ToArray())
                RemoveElement(placement.Element);
        }

        private void SetSessionMuted()
        {
            ClearElements();
            DrawElements();
        }
    }

    public static class DevicePathMapper
    {
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern uint QueryDosDevice([In] string lpDeviceName, [Out] StringBuilder lpTargetPath, [In] int ucchMax);

        public static string FromDevicePath(string devicePath)
        {
            var drive = Array.Find(DriveInfo.GetDrives(), d => devicePath.StartsWith(d.GetDevicePath(), StringComparison.InvariantCultureIgnoreCase));
            return drive != null ? devicePath.ReplaceFirst(drive.GetDevicePath(), drive.GetDriveLetter()) : null;
        }

        private static string GetDevicePath(this DriveInfo driveInfo)
        {
            var devicePathBuilder = new StringBuilder(128);
            return QueryDosDevice(driveInfo.GetDriveLetter(), devicePathBuilder, devicePathBuilder.Capacity + 1) != 0 ? devicePathBuilder.ToString() : null;
        }

        private static string GetDriveLetter(this DriveInfo driveInfo)
        {
            return driveInfo.Name.Substring(0, 2);
        }

        private static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }

            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}