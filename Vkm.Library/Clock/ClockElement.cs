using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Library.Clock
{
    internal class ClockElement: ElementBase
    {
        public override DeviceSize ButtonCount => new DeviceSize(3, 1);

        private byte _hours = 99;
        private byte _minutes = 99;
        private byte _seconds = 99;
        
        public ClockElement(Identifier id) : base(id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            RegisterTimer(new TimeSpan(0,0,0,1), () =>  DrawElementInvoke(ProvideTime()));
        }

        public override void EnterLayout(LayoutContext layoutContext)
        {
            base.EnterLayout(layoutContext);

            DrawElementInvoke(ProvideTime());
        }

        public override void LeaveLayout()
        {
            base.LeaveLayout();

            _hours = 99;
            _minutes = 99;
            _seconds = 99;
        }

        IEnumerable<LayoutDrawElement> ProvideTime()
        {
            var now = DateTime.Now;

            if (_hours != now.Hour)
            {
                _hours = (byte)now.Hour;
                yield return new LayoutDrawElement(new Location(0, 0), DrawNumber(_hours));
            }

            if (_minutes != now.Minute)
            {
                _minutes = (byte)now.Minute;
                yield return new LayoutDrawElement(new Location(1, 0), DrawNumber(_minutes));
            }

            if (_seconds != now.Second)
            {
                _seconds = (byte)now.Second;
                yield return new LayoutDrawElement(new Location(2, 0), DrawNumber(_seconds));
            }
        }

        private Bitmap DrawNumber(byte number)
        {
            var bitmap = LayoutContext.CreateBitmap();

            var fontFamily = GlobalContext.Options.Theme.FontFamily;

            var height = FontEstimation.EstimateFontSize(bitmap, fontFamily, "88");

            using (var graphics = Graphics.FromImage(bitmap))
            using (var whiteBrush = new SolidBrush(GlobalContext.Options.Theme.ForegroundColor))
            using (var font = new Font(fontFamily, height, GraphicsUnit.Pixel))
            {
                var str = number.ToString("00");
                var size = graphics.MeasureString(str, font);

                graphics.DrawString(str, font, whiteBrush, (bitmap.Width - size.Width)/2, (bitmap.Height - size.Height)/2);

            }

            return bitmap;
        }
    }
}
