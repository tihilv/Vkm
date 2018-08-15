using System.Diagnostics;
using System.Drawing;
using Vkm.Api;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
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
            var result = new RunOptions();

            if (Id == CompositeLayout.CompositeLayout.CalcIdentifier)
            {
                result.Executable = "calc.exe";
                result.Symbol = FontAwesomeRes.fa_calculator;
            }

            return result;
        }

        public void InitOptions(IOptions options)
        {
            _options = (RunOptions) options;
        }

        public override void EnterLayout(LayoutContext layoutContext)
        {
            base.EnterLayout(layoutContext);

            DrawElementInvoke(new [] {new LayoutDrawElement(new Location(0, 0), Draw())});
        }

        private Bitmap Draw()
        {
            var bitmap = LayoutContext.CreateBitmap();

            var fontFamily = FontService.Instance.AwesomeFontFamily;

            var height = FontEstimation.EstimateFontSize(bitmap, fontFamily, _options.Symbol);

            using (var graphics = Graphics.FromImage(bitmap))
            using (var whiteBrush = new SolidBrush(GlobalContext.Options.Theme.ForegroundColor))
            using (var font = new Font(fontFamily, height, GraphicsUnit.Pixel))
            {
                var size = graphics.MeasureString(_options.Symbol, font);
                graphics.DrawString(_options.Symbol, font, whiteBrush, (bitmap.Width - size.Width)/2, (bitmap.Height - size.Height)/2);
            }

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
