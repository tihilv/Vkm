using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Common;
using Vkm.Api.Data;
using Vkm.Api.Drawable;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;
using Vkm.Library.Interfaces.Service.WeatherStation;

namespace Vkm.Library.WeatherStation
{
    class WeatherStationMeasurementDetailsLayout: ILayout, IInitializable
    {
        private readonly WeatherStationDataSource _element;
        private readonly IWeatherStationService _weatherStationService;
        private GlobalContext _globalContext;
        private LayoutContext _layoutContext;

        public Identifier Id { get; }
        
        public byte? PreferredBrightness => null;

        public event EventHandler<DrawEventArgs> DrawRequested;

        public WeatherStationMeasurementDetailsLayout(WeatherStationDataSource element, IWeatherStationService weatherStationService)
        {
            _element = element;
            _weatherStationService = weatherStationService;
        }

        public void InitContext(GlobalContext context)
        {
            _globalContext = context;
        }

        public void Init()
        {
        }
        
        public void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            _layoutContext = layoutContext;
            ProcessDraw();
        }

        public void LeaveLayout()
        {
        }

        public void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
        {
            if (buttonEvent == ButtonEvent.Down)
                _layoutContext.SetPreviousLayout();
        }

        private async void ProcessDraw()
        {
            try
            {
                List<LayoutDrawElement> result = new List<LayoutDrawElement>(14);

                var weather = await _weatherStationService.GetHistorical();

                var values = weather.Union(new[] {await _weatherStationService.GetData()}).Select(m => _element.GetFunc(m)).ToArray();

                var currentValue = values.Last();
                var minValue = values.Min();
                var maxValue = values.Max();
                var avgValue = values.Average();

                var color = _layoutContext.Options.Theme.ForegroundColor;

                result.Add(GetHeaderElement(0, 0, "Min", color));
                result.Add(GetHeaderElement(1, 0, "Avg", color));
                result.Add(GetHeaderElement(2, 0, "Max", color));
                result.Add(GetHeaderElement(3, 0, "Cur", color));

                result.Add(GetHeaderElement(0, 1, _element.TransformFunc(minValue), color));
                result.Add(GetHeaderElement(1, 1, _element.TransformFunc(avgValue), color));
                result.Add(GetHeaderElement(2, 1, _element.TransformFunc(maxValue), color));
                result.Add(GetHeaderElement(3, 1, _element.TransformFunc(currentValue), color));

                result.Add(GetHeaderElement(4, 1, _element.Suffix, color));

                var deviceWidth = _layoutContext.ButtonCount.Width;
                using (var bitmap = new BitmapEx((ushort)(_layoutContext.IconSize.Width * deviceWidth), _layoutContext.IconSize.Height))
                {
                    bitmap.MakeTransparent();
                    DefaultDrawingAlgs.DrawPlot(bitmap, color, values, 0, bitmap.Height);
                    result.AddRange(BitmapHelpers.ExtractLayoutDrawElements(bitmap, new DeviceSize(deviceWidth, 1), 0, 2, _layoutContext));
                }

                DrawRequested?.Invoke(this, new DrawEventArgs(result.ToArray()));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during weather station layout drawing: {ex}");
            }
        }

        private LayoutDrawElement GetHeaderElement(int x, int y, string text, Color color)
        {
            var bitmap = _layoutContext.CreateBitmap();
            var textFontFamily = _layoutContext.Options.Theme.FontFamily;
            DefaultDrawingAlgs.DrawText(bitmap, textFontFamily, text, color);
            return new LayoutDrawElement(new Location(x, y), bitmap);
        }
    }
}