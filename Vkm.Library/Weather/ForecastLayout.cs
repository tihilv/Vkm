using System;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using OpenWeatherMap;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Library.Common;
using Location = Vkm.Api.Basic.Location;

namespace Vkm.Library.Weather
{
    abstract class ForecastLayout: ILayout, IOptionsProvider, IInitializable
    {
        protected GlobalContext _globalContext;
        protected LayoutContext _layoutContext;

        protected WeatherOptions _weatherOptions;

        public Identifier Id { get; }
        
        public byte? PreferredBrightness => null;

        public event EventHandler<DrawEventArgs> DrawLayout;
        
        public void InitContext(GlobalContext context)
        {
            _globalContext = context;
        }

        public void Init()
        {
            
        }
        
        public void EnterLayout(LayoutContext layoutContext)
        {
            _layoutContext = layoutContext;
            ProcessDraw();
        }

        public void LeaveLayout()
        {
            
        }

        public abstract void ButtonPressed(Location location, bool isDown);

        protected abstract Task<ForecastTime[]> GetData();

        protected abstract string GetDescription(ForecastTime forecast);

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
                result[i*3+1] = new LayoutDrawElement(new Location(i, 1), DrawTexts(WeatherService.Instance.TempToStr(weather[i].Temperature.Max), WeatherService.Instance.TempToStr(weather[i].Temperature.Min), _layoutContext));
                result[i*3+2] = new LayoutDrawElement(new Location(i, 2), DrawTexts(((int)weather[i].Pressure.Value).ToString(), weather[i].Humidity.Value.ToString()+"%", _layoutContext));
            }

            DrawLayout?.Invoke(this, new DrawEventArgs(result));
        }

        internal static Bitmap DrawIcon(ForecastTime response, string text, LayoutContext layoutContext)
        {
            var symbol = WeatherService.Instance.GetWeatherSymbol(response.Symbol);
            var bitmap = layoutContext.CreateBitmap();

            DefaultDrawingAlgs.DrawIconAndText(bitmap, WeatherService.Instance.WeatherFontFamily, symbol, layoutContext.Options.Theme.FontFamily, text, "22:22", layoutContext.Options.Theme.ForegroundColor);

            return bitmap;
        }

        internal static Bitmap DrawTexts(string l1, string l2, LayoutContext layoutContext)
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

        protected override async Task<ForecastTime[]> GetData()
        {
            return (await WeatherService.Instance.GetForecast(_weatherOptions.OpenWeatherApiKey, _weatherOptions.Place, _layoutContext.ButtonCount.Width)).Forecast;
        }

        protected override string GetDescription(ForecastTime forecast)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(int)forecast.Day.DayOfWeek];
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

        protected override async Task<ForecastTime[]> GetData()
        {
            return await WeatherService.Instance.GetForecastForDay(_weatherOptions.OpenWeatherApiKey, _weatherOptions.Place, _dateTime);
        }

        protected override string GetDescription(ForecastTime forecast)
        {
            return $"{DateTime.SpecifyKind(forecast.From, DateTimeKind.Utc).ToLocalTime().Hour}:00";
        }
    }
}
