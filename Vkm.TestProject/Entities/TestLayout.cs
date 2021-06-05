using System;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Drawable;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Transition;

namespace Vkm.TestProject.Entities
{
    public class TestLayout: ILayout
    {
        public static BitmapEx _testBitmap = new BitmapEx(10, 10);
        
        public bool LayoutEntered { get; private set; }
        public bool LayoutLeaved { get; private set; }
        
        public Identifier Id { get; private set; }

        public byte? PreferredBrightness { get; }

        public event EventHandler<DrawEventArgs> DrawRequested;
        
        internal event EventHandler<Tuple<Location, ButtonEvent>> OnButtonPressed;

        public TestLayout(Identifier id)
        {
            Id = id;
            LayoutLeaved = true;
        }

        public void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            if (LayoutEntered)
                throw new Exception("Layout already entered.");

            LayoutEntered = true;
            LayoutLeaved = false;
        }

        public void LeaveLayout()
        {
            if (LayoutLeaved)
                throw new Exception("Layout already leaved.");

            LayoutLeaved = true;
            LayoutEntered = false;
        }

        public void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
        {
            OnButtonPressed?.Invoke(this, Tuple.Create(location, buttonEvent));
        }

        public void DoDraw(Location location)
        {
            DrawRequested?.Invoke(this,
                new DrawEventArgs(new[]
                {
                    new LayoutDrawElement(location, _testBitmap.Clone(), new TransitionInfo(TransitionType.Instant, TimeSpan.Zero)),
                }));
        }
    }
}