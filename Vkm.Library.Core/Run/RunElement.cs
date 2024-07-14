using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Common;
using Vkm.Api.Data;
using Vkm.Api.Drawable;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Common;
using Vkm.Library.Common;
using Vkm.Library.Interfaces.Service;
using Vkm.Library.Interfaces.Services;

namespace Vkm.Library.Run
{
    class RunElement: ElementBase, IOptionsProvider
    {
        private IBitmapDownloadService _bitmapDownloadService;
        private IProcessService _processService;
        private RunOptions _options;
        private int _processId;
        private bool _selected;

        public override DeviceSize ButtonCount => new DeviceSize(1,1);

        public RunElement(Identifier identifier) : base(identifier)
        {
        }

        public override void Init()
        {
            base.Init();

            _processService = GlobalContext.GetServices<IProcessService>().First();
            _bitmapDownloadService = GlobalContext.GetServices<IBitmapDownloadService>().First();
        }

        public IOptions GetDefaultOptions()
        {
            return _options??new RunOptions();
        }

        public void InitOptions(IOptions options)
        {
            _options = (RunOptions) options;
        }

        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
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
                using (var iconRepresentation = _bitmapDownloadService.GetBitmapForExecutable(_options.Executable).Result)
                using (var iconBmpEx = iconRepresentation.CreateBitmap())
                {
                    BitmapHelpers.ResizeBitmap(iconBmpEx, bitmap);
                    if (_selected)
                        DefaultDrawingAlgs.SelectElement(bitmap, GlobalContext.Options.Theme);
                }
            }

            return bitmap;
        }

        public override void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
        {
            if (buttonEvent == ButtonEvent.Down)
            {
                if (_processId == 0 || !_processService.Activate(_processId))
                    _processId = _processService.Start(_options.Executable);
            }
            else if (buttonEvent == ButtonEvent.LongPress)
                _processService.Stop(_processId);
        }

        public void SetRunning(int processId, bool selected)
        {
            _processId = processId;
            _selected = selected;
        }
    }
}
