using System;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Library.Common;
using Vkm.Library.Interfaces.Service.Weather;
using Location = Vkm.Api.Basic.Location;

namespace Vkm.Library.Weather
{
    class WeatherElement: ElementBase, IOptionsProvider
    {
        private WeatherOptions _weatherOptions;

        private IWeatherService _weatherService;

        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public WeatherElement(Identifier identifier) : base(identifier)
        {
        }

        public IOptions GetDefaultOptions()
        {
            return new WeatherOptions();
        }

        public void InitOptions(IOptions options)
        {
            _weatherOptions = (WeatherOptions) options;
        }

        public override void Init()
        {
            base.Init();

            _weatherService = GlobalContext.GetServices<IWeatherService>().FirstOrDefault();
            if (_weatherService == null)
                throw new ApplicationException("Weather service is not available.");

            RegisterTimer(new TimeSpan(0,0,5,0), ProcessDraw);
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            ProcessDraw();
        }

        private async void ProcessDraw()
        {
            try
            {
                WeatherInfo weather = await _weatherService.GetCurrentWeather(_weatherOptions.Place);
                var img = Draw(weather, LayoutContext);
                DrawInvoke(new[] {new LayoutDrawElement(new Location(0, 0), img)});
            }
            catch (Exception)
            {
                var bitmap = LayoutContext.CreateBitmap();
                DefaultDrawingAlgs.DrawIconAndText(bitmap, GlobalContext.Options.Theme.FontFamily, "...", GlobalContext.Options.Theme.FontFamily, "", "88888", GlobalContext.Options.Theme.ForegroundColor);
                DrawInvoke(new[] {new LayoutDrawElement(new Location(0, 0), bitmap)});
            }
        }

        private static BitmapEx Draw(WeatherInfo weatherInfo, LayoutContext layoutContext)
        {
            var temperature = WeatherHelpers.TempToStr(weatherInfo.TemperatureCelsius??0);
            var symbol = WeatherHelpers.GetWeatherSymbol(weatherInfo.Symbol);
            
            var bitmap = layoutContext.CreateBitmap();

            DefaultDrawingAlgs.DrawIconAndText(bitmap, WeatherHelpers.WeatherFontFamily, symbol, layoutContext.Options.Theme.FontFamily, temperature, "+88", layoutContext.Options.Theme.ForegroundColor);

            return bitmap;
        }

        public override bool ButtonPressed(Location location, bool isDown)
        {
            if (isDown)
            {
                var forecastLayout = new DaysForecastLayout();
                GlobalContext.InitializeEntity(forecastLayout);
                forecastLayout.InitOptions(_weatherOptions);

                LayoutContext.SetLayout(forecastLayout);
            }
            return base.ButtonPressed(location, isDown);
        }
    }
}
