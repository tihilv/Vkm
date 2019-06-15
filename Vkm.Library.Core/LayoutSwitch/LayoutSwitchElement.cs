using System;
using System.Drawing;
using Vkm.Api.Basic;
using Vkm.Api.Common;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Library.LayoutSwitch
{
    public class LayoutSwitchElement : ElementBase
    {
        private ILayout _previousLayout;
        private ILayout _layoutToManage;

        private IconSize _individualButtonSize;
        private IconSize _buttonShift;

        private BitmapRepresentation _currentRepresentation;
        
        public LayoutSwitchElement(Identifier identifier) : base(identifier)
        {
            
        }

        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public void SetData(ILayout layoutToManage, ILayout previousLayout)
        {
            _layoutToManage = layoutToManage;
            _previousLayout = previousLayout;
        }

        private IconSize CalculateIndividualButtonSize()
        {
            var maxWidth = LayoutContext.IconSize.Width / LayoutContext.ButtonCount.Width;
            var maxHeight = LayoutContext.IconSize.Height / LayoutContext.ButtonCount.Height;
            var minDimension = Math.Min(maxWidth, maxHeight);
            
            return new IconSize(minDimension, minDimension);
        }

        private IconSize CalculateButtonShift()
        {
            return new IconSize(
                (LayoutContext.IconSize.Width - LayoutContext.ButtonCount.Width * _individualButtonSize.Width) / 2,
                (LayoutContext.IconSize.Height - LayoutContext.ButtonCount.Height * _individualButtonSize.Height) / 2);
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            _individualButtonSize = CalculateIndividualButtonSize();
            _buttonShift = CalculateButtonShift();
            using (var bitmap = layoutContext.CreateBitmap())
                _currentRepresentation = new BitmapRepresentation(bitmap);
            _layoutToManage.DrawLayout += OnDrawLayout;
            _layoutToManage.EnterLayout(layoutContext, _previousLayout);
        }

        public override void LeaveLayout()
        {
            _layoutToManage.LeaveLayout();
            _layoutToManage.DrawLayout -= OnDrawLayout;
            base.LeaveLayout();
            
            DisposeHelper.DisposeAndNull(ref _currentRepresentation);
        }

        private readonly object _bitmapLock = new object();

        private void OnDrawLayout(object sender, DrawEventArgs e)
        {
            BitmapEx currentBitmap;

            lock (_bitmapLock)
            {
                currentBitmap = _currentRepresentation.CreateBitmap();
                foreach (var element in e.Elements)
                {
                    var destRect = new Rectangle(_buttonShift.Width + element.Location.X * _individualButtonSize.Width, _buttonShift.Height + element.Location.Y * _individualButtonSize.Height, _individualButtonSize.Width, _individualButtonSize.Height);
                    using (var sourceBitmap = element.BitmapRepresentation.CreateBitmap())
                    {
                        BitmapHelpers.ResizeBitmap(sourceBitmap, currentBitmap, destRect);
                        element.BitmapRepresentation.Dispose();
                    }
                }

                DisposeHelper.DisposeAndNull(ref _currentRepresentation);
                _currentRepresentation = new BitmapRepresentation(currentBitmap);
            }

            DrawInvoke(new[] {new LayoutDrawElement(new Location(0, 0), currentBitmap)});
        }

        public override bool ButtonPressed(Location location, ButtonEvent buttonEvent)
        {
            if (buttonEvent == ButtonEvent.Down)
            {
                LayoutContext.SetLayout(_layoutToManage);
                return true;
            }

            return false;
        }
    }
}