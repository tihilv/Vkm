﻿using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;
using Vkm.Library.Interfaces.Service.Remote;

namespace Vkm.Library.RemoteControl
{
    class RemoteDefaultElement : ElementBase
    {
        private readonly IRemoteControlService _service;
        private readonly ActionInfo _action;

        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public RemoteDefaultElement(Identifier identifier, ActionInfo action, IRemoteControlService service) : base(identifier)
        {
            _action = action;
            _service = service;
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);
            
            Draw();
        }

        private void Draw()
        {
            var bitmap = LayoutContext.CreateBitmap();

            var fontFamily = GlobalContext.Options.Theme.FontFamily;

            DefaultDrawingAlgs.DrawText(bitmap, fontFamily, _action.Text, GlobalContext.Options.Theme.ForegroundColor);
            
            if (_action.Active)
                DefaultDrawingAlgs.SelectElement(bitmap, GlobalContext.Options.Theme);

            DrawInvoke(new [] {new LayoutDrawElement(new Location(0, 0), bitmap) });
        }

        public override bool ButtonPressed(Location location, ButtonEvent buttonEvent)
        {
            if (buttonEvent == ButtonEvent.Down && location.X == 0 && location.Y == 0)
            {
                _service.StartAction(_action);
                
                return true;
            }
            return base.ButtonPressed(location, buttonEvent);
        }
    }
}