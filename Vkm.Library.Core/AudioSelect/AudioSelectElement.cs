using System;
using System.Collections.Generic;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Drawable;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Common;
using Vkm.Library.Common;

namespace Vkm.Library.AudioSelect
{
    class AudioSelectElement: ElementBase, IOptionsProvider
    {
        private AudioSelectOptions _options;

        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public AudioSelectElement(Identifier identifier) : base(identifier)
        {
        }

        public IOptions GetDefaultOptions()
        {
            return new AudioSelectOptions();
        }

        public void InitOptions(IOptions options)
        {
            _options = (AudioSelectOptions) options;
        }

        
        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            var bitmap = LayoutContext.CreateBitmap();
            DefaultDrawingAlgs.DrawText(bitmap, FontService.Instance.AwesomeFontFamily, FontAwesomeRes.fa_volume_up, GlobalContext.Options.Theme.ForegroundColor);
            DrawInvoke(new[] {new LayoutDrawElement(new Location(0, 0), bitmap)});
        }

        public override void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
        {
            if (buttonEvent == ButtonEvent.Down)
                LayoutContext.SetLayout(GlobalContext.InitializeEntity(new AudioSelectLayout(Id, _options)));


            base.ButtonPressed(location, buttonEvent, layoutContext);
        }
    }

    [Serializable]
    public class AudioSelectOptions : IOptions
    {
        private Dictionary<string, string> _names;

        public Dictionary<string, string> Names => _names;

        public AudioSelectOptions()
        {
            _names = new Dictionary<string, string>();
        }
    }
}
