using System;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Library.Common;
using Vkm.Library.Interfaces.Service.Remote;

namespace Vkm.Library.RemoteControl
{
    internal class RemoteControlLayout: LayoutBase
    {
        private IRemoteControlService _service;
        
        public RemoteControlLayout(Identifier identifier): base(identifier)
        {
            
        }

        public override void Init()
        {
            base.Init();

            _service = GlobalContext.GetServices<IRemoteControlService>().First();

            AddElement(new Location(4, 2), GlobalContext.InitializeEntity(new BackElement()));
        }

        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            _service.ActiveActionChanged += ServiceOnActiveActionChanged;

            FillData(layoutContext);
        }

        protected override void OnLeavingLayout()
        {
            _service.ActiveActionChanged -= ServiceOnActiveActionChanged;
        }

        private void ServiceOnActiveActionChanged(object sender, ActionEventArgs e)
        {
            WithLayout(l => FillData(l));
        }

        async void FillData(LayoutContext layoutContext)
        {
            var actions = await _service.GetActions();

            var elements = actions.Select(a => GlobalContext.InitializeEntity(new RemoteDefaultElement(new Identifier($"{Id.Value}_{a.Id}"), a, _service))).Take(layoutContext.ButtonCount.Width * layoutContext.ButtonCount.Height - 1);
            
            base.AddElementsInRectangle(elements);
        }
    }
}
