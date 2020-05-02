using System;
using System.Drawing;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Common;
using Vkm.Library.Interfaces.Service.Calendar;
using Location = Vkm.Api.Basic.Location;

namespace Vkm.Library.Calendar
{
    class CalendarElement: ElementBase, IOptionsProvider
    {
        private CalendarElementOptions _options;
        
        private ICalendarService[] _calendarServices;

        private AppointmentInfo _cachedAppointment;
        private int _cachedAppointmentCount = -1;
        private byte _cachedOpacity;

        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public CalendarElement(Identifier identifier) : base(identifier)
        {
        }

        public override void Init()
        {
            base.Init();

            RegisterTimer(new TimeSpan(0,0,0,5), ProcessDraw);

            _calendarServices = GlobalContext.GetServices<ICalendarService>().ToArray();
        }

        AppointmentInfo[] GetUpcomingEvents()
        {
            var from = DateTime.Now.Subtract(_options.ExpiredMeetingTolerance);
            var to = from.AddDays(2).Date;

            var events = _calendarServices.SelectMany(c => c.GetCalendar(from, to)).OrderBy(c=>c.FromDateTime).ToArray();

            return events;
        }

        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            ProcessDraw();
        }

        protected override void OnLeavedLayout()
        {
            _cachedAppointment = null;
            _cachedAppointmentCount = -1;
            _cachedOpacity = 0;
        }

        byte GetOpacity(AppointmentInfo appointment)
        {
            if (appointment == null)
                return byte.MinValue;
            
            var delta = appointment.FromDateTime - DateTime.Now;

            if (_options.UpcomingMeetingNotificationPeriod < delta)
                return byte.MinValue;

            if (delta.TotalMilliseconds < 0)
                return byte.MaxValue;
            
            return (byte)(byte.MaxValue * (_options.UpcomingMeetingNotificationPeriod.TotalMilliseconds - delta.TotalMilliseconds) / (_options.UpcomingMeetingNotificationPeriod.TotalMilliseconds));
        }
        
        private void ProcessDraw()
        {
            var result = GetUpcomingEvents();
            var nextAppointment = result.FirstOrDefault();
            
            var opacity = GetOpacity(nextAppointment);
            if (nextAppointment != _cachedAppointment || result.Length != _cachedAppointmentCount || opacity != _cachedOpacity)
            {
                var img = Draw(nextAppointment, opacity, LayoutContext);
                _cachedAppointment = nextAppointment;
                _cachedAppointmentCount = result.Length;
                _cachedOpacity = opacity;
                DrawInvoke(new[] {new LayoutDrawElement(new Location(0, 0), img)});
            }
        }

        private BitmapEx Draw(AppointmentInfo nextAppointment, byte opacity, LayoutContext layoutContext)
        {
            var bitmap = layoutContext.CreateBitmap();

            if (opacity != 0)
            {
                using (var graphics = bitmap.CreateGraphics())
                using (var brush = new SolidBrush(Color.FromArgb(opacity, _options.UpcomingMeetingColor)))
                {
                    graphics.FillRectangle(brush, 0,0, bitmap.Width, bitmap.Height);
                }
            }
            
            string result;
            result = nextAppointment != null ? $"{nextAppointment.FromDateTime:t}\n{nextAppointment.Location?.Split('(')[0].Trim()}" : "X";
            
            DefaultDrawingAlgs.DrawText(bitmap, layoutContext.Options.Theme.FontFamily, result, layoutContext.Options.Theme.ForegroundColor);

            return bitmap;
        }

        public override bool ButtonPressed(Location location, ButtonEvent buttonEvent)
        {
            if (buttonEvent == ButtonEvent.Down)
            {
                var weatherStationLayout = new CalendarLayout(_options);
                GlobalContext.InitializeEntity(weatherStationLayout);

                LayoutContext.SetLayout(weatherStationLayout);
                
                return true;
            }
            return base.ButtonPressed(location, buttonEvent);
        }

        public IOptions GetDefaultOptions()
        {
            return new CalendarElementOptions();
        }

        public void InitOptions(IOptions options)
        {
            _options = (CalendarElementOptions) options;
        }
    }
}
