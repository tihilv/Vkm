using System;
using System.Drawing;
using OpenWeatherMap;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Library.Common;
using Location = Vkm.Api.Basic.Location;

namespace Vkm.Library.Weather
{
    class WeatherElement: ElementBase, IOptionsProvider
    {
        private WeatherOptions _weatherOptions;

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
                var service = WeatherService.Instance;
                var weather = await service.GetWeather(_weatherOptions.OpenWeatherApiKey, _weatherOptions.Place);
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

        internal static BitmapEx Draw(WeatherItem response, LayoutContext layoutContext)
        {
            var temperature = WeatherService.Instance.TempToStr(response.Temperature.Value);
            var symbol = WeatherService.Instance.GetWeatherSymbol(response.Weather);
            
            var bitmap = layoutContext.CreateBitmap();

            DefaultDrawingAlgs.DrawIconAndText(bitmap, WeatherService.Instance.WeatherFontFamily, symbol, layoutContext.Options.Theme.FontFamily, temperature, "+88", layoutContext.Options.Theme.ForegroundColor);

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
