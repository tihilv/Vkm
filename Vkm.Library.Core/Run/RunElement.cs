using System;
using System.Linq;
using Vkm.Api.Basic;
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
            return new RunOptions();
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

            var fontFamily = FontService.Instance.AwesomeFontFamily;

            DefaultDrawingAlgs.DrawText(bitmap, fontFamily, _options.Symbol, GlobalContext.Options.Theme.ForegroundColor);

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
    }
}
