using System.Diagnostics;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Common;
using Vkm.Common.Win32.Win32;
using Vkm.Library.Common;

namespace Vkm.Library.Run
{
    class RunElement: ElementBase, IOptionsProvider
    {
        private RunOptions _options;
        private Process _process;
        
        public override DeviceSize ButtonCount => new DeviceSize(1,1);

        public RunElement(Identifier identifier) : base(identifier)
        {
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
                if (_process == null || _process.HasExited)
                {
                    _process = Process.Start(_options.Executable);
                }
                else
                {
                    Win32.SwitchToThisWindow(_process.MainWindowHandle, true);
                }
                return true;
            }

            return base.ButtonPressed(location, isDown);
        }
    }
}
