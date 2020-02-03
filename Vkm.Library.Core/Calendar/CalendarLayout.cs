using System;
using System.Diagnostics;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Common;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Api.Time;
using Vkm.Common;
using Vkm.Library.Interfaces.Service.Calendar;

namespace Vkm.Library.Calendar
{
    class CalendarLayout: ILayout, IInitializable, IOptionsProvider
    {
        private CalendarElementOptions _options;
        
        private GlobalContext _globalContext;
        private LayoutContext _layoutContext;

        private ICalendarService[] _calendarServices;

        private AppointmentInfo[] _appointments;
        
        private ITimerToken _timerToken;

        public Identifier Id { get; }
        
        public byte? PreferredBrightness => null;

        public event EventHandler<DrawEventArgs> DrawLayout;

        public CalendarLayout()
        {
            
        }

        public CalendarLayout(CalendarElementOptions options)
        {
            _options = options;
        }

        public void InitContext(GlobalContext context)
        {
            _globalContext = context;
            _timerToken = _globalContext.Services.TimerService.RegisterTimer(new TimeSpan(0, 0, 5, 0), ProcessDraw);
        }

        public void Init()
        {
            _calendarServices = _globalContext.GetServices<ICalendarService>().ToArray();
        }
        
        public void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            _layoutContext = layoutContext;
            ProcessDraw();
            _timerToken.Start();
        }

        public void LeaveLayout()
        {
            _timerToken.Stop();
        }

        public void ButtonPressed(Location location, ButtonEvent buttonEvent)
        {
            if (buttonEvent == ButtonEvent.Down)
            {
                if (location.X == 0 && _appointments.Length > location.Y)
                {
                    foreach (var calendarService in _calendarServices)
                    {
                        if (calendarService.TryShowAppointment(_appointments[location.Y]))
                            break;
                    }
                }
                else
                    _layoutContext.SetPreviousLayout();
            }
        }

        private string FormatDateTime(DateTime fromDateTime, DateTime toDateTime)
        {
            return $"{fromDateTime:t}\n{toDateTime:t}\n{fromDateTime:dd-MM-yy}";
        }
        
        private async void ProcessDraw()
        {
            try
            {
                var from = DateTime.Now.Subtract(_options.ExpiredMeetingTolerance);
                var to = from.AddDays(2).Date;

                _appointments = _calendarServices.SelectMany(c => c.GetCalendar(from, to)).OrderBy(c=>c.FromDateTime).ToArray();

                var rows = _layoutContext.ButtonCount.Height;
                var columns = _layoutContext.ButtonCount.Width;
                LayoutDrawElement[] result = new LayoutDrawElement[columns * rows];

                for (byte i = 0; i < Math.Min(_layoutContext.ButtonCount.Height, _appointments.Length); i++)
                {
                    result[i * columns] = new LayoutDrawElement(new Location(0, i), DrawText(FormatDateTime(_appointments[i].FromDateTime, _appointments[i].ToDateTime), _layoutContext));
                    result[i * columns+1] = new LayoutDrawElement(new Location(2, i), DrawText(_appointments[i].Location, _layoutContext));
                    result[i * columns+2] = new LayoutDrawElement(new Location(1, i), DrawText(_appointments[i].Organiser, _layoutContext));

                    byte firstNameIndex = 3;
                    byte textButtonCount = (byte)(columns - firstNameIndex);
                    if (textButtonCount > 0)
                    {
                        using (var bitmap = new BitmapEx(_layoutContext.IconSize.Width * textButtonCount, _layoutContext.IconSize.Height))
                        {
                            bitmap.MakeTransparent();
                            DefaultDrawingAlgs.DrawText(bitmap, _layoutContext.Options.Theme.FontFamily, _appointments[i].Subject, _layoutContext.Options.Theme.ForegroundColor);
                            var elements = BitmapHelpers.ExtractLayoutDrawElements(bitmap, new DeviceSize(textButtonCount, 1), firstNameIndex, i, _layoutContext);
                            int j = firstNameIndex;
                            foreach (var e in elements)
                            {
                                result[i * columns + j] = e;
                                j++;
                            }
                        }
                    }

                }
                
                DrawLayout?.Invoke(this, new DrawEventArgs(result));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during weather station layout drawing: {ex}");
            }
        }

        private static BitmapEx DrawText(string l, LayoutContext layoutContext)
        {
            var bitmap = layoutContext.CreateBitmap();

            var textFontFamily = layoutContext.Options.Theme.FontFamily;

            DefaultDrawingAlgs.DrawText(bitmap, textFontFamily, l, layoutContext.Options.Theme.ForegroundColor);

            return bitmap;
        }
        
        public IOptions GetDefaultOptions()
        {
            return _options??new CalendarElementOptions();
        }

        public void InitOptions(IOptions options)
        {
            _options = (CalendarElementOptions) options;
        }

    }
}