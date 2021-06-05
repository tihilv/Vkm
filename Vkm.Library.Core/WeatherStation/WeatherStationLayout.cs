using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Time;
using Vkm.Common;
using Vkm.Library.Interfaces.Service.WeatherStation;
using Vkm.Library.Weather;

namespace Vkm.Library.WeatherStation
{
    class WeatherStationLayout: ILayout, IInitializable
    {
        private GlobalContext _globalContext;
        private LayoutContext _layoutContext;

        private readonly List<WeatherStationDataSource> _elements;
        
        private IWeatherStationService _weatherStationService;
        
        private ITimerToken _timerToken;

        public Identifier Id { get; }
        
        public byte? PreferredBrightness => null;

        public event EventHandler<DrawEventArgs> DrawLayout;

        public WeatherStationLayout()
        {
            _elements = new List<WeatherStationDataSource>();
        }

        public void InitContext(GlobalContext context)
        {
            _globalContext = context;
            _timerToken = _globalContext.Services.TimerService.RegisterTimer(new TimeSpan(0, 0, 5, 0), ProcessDraw);
        }

        public void Init()
        {
            _weatherStationService = _globalContext.GetServices<IWeatherStationService>().FirstOrDefault();
            if (_weatherStationService == null)
                throw new ApplicationException("Weather station service is not available.");
            
            _elements.Add(new WeatherStationDataSource(new Location(0,0), s=>s.Devices[0].Temperature.Value, d => WeatherHelpers.TempToDecStr(d), "     °C"));
            _elements.Add(new WeatherStationDataSource(new Location(1,0), s=>s.Devices[0].HumidityPercent.Value, d => (d/100).ToString("P0"), ""));
            _elements.Add(new WeatherStationDataSource(new Location(2,0), s=>s.Devices[0].Pressure.Value, d => d.ToString("F1"), "mmHg"));
            _elements.Add(new WeatherStationDataSource(new Location(3,0), s=>s.Devices[0].Co2Measure.Value, d => d.ToString("F0"), "ppm"));
            _elements.Add(new WeatherStationDataSource(new Location(4,0), s=>s.Devices[0].Noise.Value, d => d.ToString("F0"), "dB"));
            
            _elements.Add(new WeatherStationDataSource(new Location(0,1), s=>s.Devices[1].Temperature.Value, d => WeatherHelpers.TempToDecStr(d), "     °C"));
            _elements.Add(new WeatherStationDataSource(new Location(1,1), s=>s.Devices[1].HumidityPercent.Value, d => (d/100).ToString("P0"), ""));
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

        public void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
        {
            if (buttonEvent == ButtonEvent.Down)
            {
                var element = _elements.FirstOrDefault(e => e.Location == location);

                if (element != null)
                {
                    var detailsLayout = new WeatherStationMeasurementDetailsLayout(element, _weatherStationService);
                    _globalContext.InitializeEntity(detailsLayout);
                    _layoutContext.SetLayout(detailsLayout);
                }
                else
                {
                    _layoutContext.SetPreviousLayout();
                }
            }
        }

        private async void ProcessDraw()
        {
            try
            {
                var weather = await _weatherStationService.GetHistorical();

                LayoutDrawElement[] result = (new LayoutDrawElement[_elements.Count]);

                for (int i = 0; i < _elements.Count; i++)
                {
                    var element = _elements[i];

                    var values = weather.Union(new[] {await _weatherStationService.GetData()}).Select(m => element.GetFunc(m)).ToArray();

                    var currentValue = values.Last();
                    var currentString = element.TransformFunc(currentValue);

                    result[i] = new LayoutDrawElement(element.Location, DrawTexts(currentString, element.Suffix, values, _layoutContext));
                }

                DrawLayout?.Invoke(this, new DrawEventArgs(result));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during weather station layout drawing: {ex}");
            }
        }

        private static BitmapEx DrawTexts(string l1, string l2, double[] values, LayoutContext layoutContext)
        {
            var bitmap = layoutContext.CreateBitmap();

            var textFontFamily = layoutContext.Options.Theme.FontFamily;

            DefaultDrawingAlgs.DrawTexts(bitmap, textFontFamily, l1 + Environment.NewLine + l2, "", "DDDD", layoutContext.Options.Theme.ForegroundColor);

            var startIndex = 0;
            if (values.Length > bitmap.Width)
                startIndex = values.Length - bitmap.Width;

            DefaultDrawingAlgs.DrawPlot(bitmap, layoutContext.Options.Theme.ForegroundColor, values, bitmap.Height / 2, bitmap.Height, startIndex);

            return bitmap;
        }
    }
}