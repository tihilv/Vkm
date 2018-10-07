using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Common;
using Vkm.Library.Interfaces.Service.Weather;
using Location = Vkm.Api.Basic.Location;

namespace Vkm.Library.Weather
{
    abstract class ForecastLayout: ILayout, IOptionsProvider, IInitializable
    {
        protected GlobalContext _globalContext;
        protected LayoutContext _layoutContext;

        protected WeatherOptions _weatherOptions;

        protected IWeatherService _weatherService;

        public Identifier Id { get; }
        
        public byte? PreferredBrightness => null;

        public event EventHandler<DrawEventArgs> DrawLayout;
        
        public void InitContext(GlobalContext context)
        {
            _globalContext = context;
        }

        public void Init()
        {
            _weatherService = _globalContext.GetServices<IWeatherService>().FirstOrDefault();
            if (_weatherService == null)
                throw new ApplicationException("Weather service is not available.");
        }
        
        public void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            _layoutContext = layoutContext;
            ProcessDraw();
        }

        public void LeaveLayout()
        {
            
        }

        public abstract void ButtonPressed(Location location, bool isDown);

        protected abstract Task<WeatherInfo[]> GetData();

        protected abstract string GetDescription(WeatherInfo forecast);

        public IOptions GetDefaultOptions()
        {
            return new WeatherOptions();
        }

        public void InitOptions(IOptions options)
        {
            _weatherOptions = (WeatherOptions) options;
        }
        
        private async void ProcessDraw()
        {
            var weather = await GetData();

            LayoutDrawElement[] result = new LayoutDrawElement[_layoutContext.ButtonCount.Width*3];

            for (byte i = 0; i < Math.Min(_layoutContext.ButtonCount.Width, weather.Length); i++)
            {
                result[i*3] = new LayoutDrawElement(new Location(i, 0), DrawIcon(weather[i], GetDescription(weather[i]), _layoutContext));
                result[i*3+1] = new LayoutDrawElement(new Location(i, 1), DrawTexts(WeatherHelpers.TempToStr(weather[i].TemperatureMaxCelsius??0), WeatherHelpers.TempToStr(weather[i].TemperatureMinCelsius??0), _layoutContext));
                result[i*3+2] = new LayoutDrawElement(new Location(i, 2), DrawTexts(((int)(weather[i].PressureMPa??0)).ToString(), weather[i].Humidity.ToString()+"%", _layoutContext));
            }

            DrawLayout?.Invoke(this, new DrawEventArgs(result));
        }

        internal static BitmapEx DrawIcon(WeatherInfo response, string text, LayoutContext layoutContext)
        {
            var symbol = WeatherHelpers.GetWeatherSymbol(response.Symbol);
            var bitmap = layoutContext.CreateBitmap();

            DefaultDrawingAlgs.DrawIconAndText(bitmap, WeatherHelpers.WeatherFontFamily, symbol, layoutContext.Options.Theme.FontFamily, text, "22:22", layoutContext.Options.Theme.ForegroundColor);

            return bitmap;
        }

        internal static BitmapEx DrawTexts(string l1, string l2, LayoutContext layoutContext)
        {
            var bitmap = layoutContext.CreateBitmap();

            var textFontFamily = layoutContext.Options.Theme.FontFamily;

            DefaultDrawingAlgs.DrawTexts(bitmap, textFontFamily, l1, l2, "DDD", layoutContext.Options.Theme.ForegroundColor);

            return bitmap;
        }
    }

    class DaysForecastLayout : ForecastLayout
    {
        public override void ButtonPressed(Location location, bool isDown)
        {
            if (isDown)
            {
                if (location.Y == 0)
                {
                    var forecastLayout = new HoursForecastLayout(DateTime.Now.Date.AddDays(location.X));
                    _globalContext.InitializeEntity(forecastLayout);
                    forecastLayout.InitOptions(_weatherOptions);
                    _layoutContext.SetLayout(forecastLayout);

                }
                else
                {
                    _layoutContext.SetPreviousLayout();
                }
                
            }
        }

        protected override async Task<WeatherInfo[]> GetData()
        {
            return (await _weatherService.GetForecast(_weatherOptions.Place, _layoutContext.ButtonCount.Width));
        }

        protected override string GetDescription(WeatherInfo forecast)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(int)forecast.DateTime.DayOfWeek];
        }
    }

    class HoursForecastLayout : ForecastLayout
    {
        private readonly DateTime _dateTime;

        public HoursForecastLayout(DateTime dateTime)
        {
            _dateTime = dateTime;
        }

        public override void ButtonPressed(Location location, bool isDown)
        {
            if (isDown)
            {
                _layoutContext.SetPreviousLayout();
            }
        }

        protected override async Task<WeatherInfo[]> GetData()
        {
            return await _weatherService.GetForecastForDay(_weatherOptions.Place, _dateTime);
        }

        protected override string GetDescription(WeatherInfo forecast)
        {
            return $"{DateTime.SpecifyKind(forecast.DateTime, DateTimeKind.Utc).ToLocalTime().Hour}:00";
        }
    }
}
