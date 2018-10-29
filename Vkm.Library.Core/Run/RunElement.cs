using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Common;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Common;
using Vkm.Library.Common;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.Run
{
    class RunElement: ElementBase, IOptionsProvider
    {
        private IProcessService _processService;
        private RunOptions _options;
        private IntPtr _handle;
        
        public override DeviceSize ButtonCount => new DeviceSize(1,1);

        public RunElement(Identifier identifier) : base(identifier)
        {
        }

        public override void Init()
        {
            base.Init();

            _processService = GlobalContext.GetServices<IProcessService>().First();
        }

        public IOptions GetDefaultOptions()
        {
            return _options??new RunOptions();
        }

        public void InitOptions(IOptions options)
        {
            _options = (RunOptions) options;
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            DrawInvoke(new [] {new LayoutDrawElement(new Location(0, 0), Draw())});
        }

        private BitmapEx Draw()
        {
            var bitmap = LayoutContext.CreateBitmap();

            if (!string.IsNullOrEmpty(_options.Symbol))
            {
                var fontFamily = FontService.Instance.AwesomeFontFamily;
                DefaultDrawingAlgs.DrawText(bitmap, fontFamily, _options.Symbol, GlobalContext.Options.Theme.ForegroundColor);
            }
            else
            {
                using (var icon = Icon.ExtractAssociatedIcon(_options.Executable))
                using (var iconBmp = icon.ToBitmap())
                using (var iconRepresentation = new BitmapRepresentation(iconBmp))
                using (var iconBmpEx = iconRepresentation.CreateBitmap())
                {
                    BitmapHelpers.ResizeBitmap(iconBmpEx, bitmap);
                }
            }

            return bitmap;
        }

        public override bool ButtonPressed(Location location, bool isDown)
        {
            if (isDown)
            {
                if (_handle == IntPtr.Zero || !_processService.Activate(_handle))
                    _handle = _processService.Start(_options.Executable);

                return true;
            }

            return base.ButtonPressed(location, isDown);
        }

        public void SetRunning(IntPtr handle)
        {
            _handle = handle;
        }
    }
}
