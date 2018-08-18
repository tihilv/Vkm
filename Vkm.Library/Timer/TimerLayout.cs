using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Library.Common;

namespace Vkm.Library.Timer
{
    class TimerLayout: LayoutBase
    {
        public TimerLayout(Identifier id) : base(id)
        {
        }

        public override void Init()
        {
            base.Init();
            
            AddElement(new Location(0,0), GlobalContext.InitializeEntity(new CountdownElement(new Identifier("Vkm.Countdown"))));
            AddElement(new Location(0,1), GlobalContext.InitializeEntity(new StopwatchElement(new Identifier("Vkm.Stopwatch"))));
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            DrawCommon();
        }

        private void DrawCommon()
        {
            var bmp3 = LayoutContext.CreateBitmap();
            DefaultDrawingAlgs.DrawText(bmp3, FontService.Instance.AwesomeFontFamily, FontAwesomeRes.fa_arrow_left, FontAwesomeRes.fa_arrow_left, GlobalContext.Options.Theme.ForegroundColor);

            DrawInvoke(new[] { new LayoutDrawElement(new Location(4, 2), bmp3)});
        }

        public override void ButtonPressed(Location location, bool isDown)
        {
            if (isDown)
            {
                if (location.X == 4 && location.Y == 2)
                {
                    LayoutContext.SetPreviousLayout();
                }
            }

            base.ButtonPressed(location, isDown);
        }

        

        
    }
}
