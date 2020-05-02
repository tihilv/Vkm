using System;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;
using Vkm.Common;
using Vkm.Library.Common;

namespace Vkm.Library.AudioSessions
{
    class AudioSessionsElement: ElementBase, IOptionsProvider
    {
        private AudioSessionsOptions _options;

        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public AudioSessionsElement(Identifier identifier) : base(identifier)
        {
        }

        public IOptions GetDefaultOptions()
        {
            return new AudioSessionsOptions();
        }

        public void InitOptions(IOptions options)
        {
            _options = (AudioSessionsOptions) options;
        }

        
        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            var bitmap = LayoutContext.CreateBitmap();
            DefaultDrawingAlgs.DrawText(bitmap, FontService.Instance.AwesomeFontFamily, FontAwesomeRes.fa_random, GlobalContext.Options.Theme.ForegroundColor);
            DrawInvoke(new[] {new LayoutDrawElement(new Location(0, 0), bitmap)});
        }

        public override bool ButtonPressed(Location location, ButtonEvent buttonEvent)
        {
            if (buttonEvent == ButtonEvent.Down)
                LayoutContext.SetLayout(GlobalContext.InitializeEntity(new AudioSessionsLayout(Id, _options)));


            return base.ButtonPressed(location, buttonEvent);
        }
    }

    [Serializable]
    public class AudioSessionsOptions : IOptions
    {
        public AudioSessionsOptions()
        {
        }
    }
}
