using System;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;
using Vkm.Library.Interfaces.Service.WeatherStation;
using Vkm.Library.Weather;
using Location = Vkm.Api.Basic.Location;

namespace Vkm.Library.WeatherStation
{
    class WeatherStationElement: ElementBase
    {
        private IWeatherStationService _weatherStationService;

        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public WeatherStationElement(Identifier identifier) : base(identifier)
        {
        }

        public override void Init()
        {
            base.Init();

            _weatherStationService = GlobalContext.GetServices<IWeatherStationService>().FirstOrDefault();
            if (_weatherStationService == null)
                throw new ApplicationException("Weather station service is not available.");

            RegisterTimer(new TimeSpan(0,0,5,0), ProcessDraw);
        }

        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            ProcessDraw();
        }

        private async void ProcessDraw()
        {
            try
            {
                var weather = await _weatherStationService.GetData();
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

        private static BitmapEx Draw(WeatherStationData weatherInfo, LayoutContext layoutContext)
        {
            var indoorDevice = weatherInfo.Devices.First(); 
            var temperature = WeatherHelpers.TempToStr(indoorDevice.Temperature.Value);
            var co2 = indoorDevice.Co2Measure.Value.ToString();
            
            var bitmap = layoutContext.CreateBitmap();

            DefaultDrawingAlgs.DrawTexts(bitmap, layoutContext.Options.Theme.FontFamily, co2, temperature, "+88", layoutContext.Options.Theme.ForegroundColor);

            return bitmap;
        }

        public override bool ButtonPressed(Location location, ButtonEvent buttonEvent)
        {
            if (buttonEvent == ButtonEvent.Down)
            {
                var weatherStationLayout = new WeatherStationLayout();
                GlobalContext.InitializeEntity(weatherStationLayout);

                LayoutContext.SetLayout(weatherStationLayout);
            }
            return base.ButtonPressed(location, buttonEvent);
        }
    }
}
