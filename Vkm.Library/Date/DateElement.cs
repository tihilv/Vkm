using System;
using System.Globalization;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Library.Common;

namespace Vkm.Library.Date
{
    internal class DateElement: ElementBase
    {
        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        private DateTime _date = DateTime.MinValue;
        
        public DateElement(Identifier id) : base(id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            RegisterTimer(new TimeSpan(0, 0, 1, 0), Draw);
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            Draw();
        }

        public override void LeaveLayout()
        {
            base.LeaveLayout();

            _date = DateTime.MinValue;
        }

        private void Draw()
        {
            var now = DateTime.Now.Date;

            if (_date != now)
            {
                _date = now;

                DrawInvoke(
                    new[] {new LayoutDrawElement(new Location(0, 0), DrawDate(_date))}
                );
            }
        }
        
        private BitmapEx DrawDate(DateTime date)
        {
            var bitmap = LayoutContext.CreateBitmap();

            var fontFamily = GlobalContext.Options.Theme.FontFamily;

            var day = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(int) date.DayOfWeek];
            var dateStr1 = $"{day}, {date.Day}";
            var dateStr2 = $"{date.ToString("MM.yy")}";
            DefaultDrawingAlgs.DrawTexts(bitmap, fontFamily, dateStr1, dateStr2, "W00.00", GlobalContext.Options.Theme.ForegroundColor);

            return bitmap;
        }
    }
}
