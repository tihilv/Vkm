using System.Collections.Generic;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Library.Common;
using Vkm.Library.Interfaces.Service.Remote;

namespace Vkm.Library.RemoteControl
{
    internal class RemoteControlLayout: LayoutBase
    {
        private List<IElement> _elements = new List<IElement>();
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

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);
            
            _service.ActiveActionChanged += ServiceOnActiveActionChanged;

            FillData();
        }

        public override void LeaveLayout()
        {
            _service.ActiveActionChanged -= ServiceOnActiveActionChanged;
            base.LeaveLayout();
        }

        private void ServiceOnActiveActionChanged(object sender, ActionEventArgs e)
        {
            FillData();
        }

        async void FillData()
        {
            foreach (var element in _elements)
                RemoveElement(element);

            _elements.Clear();
            
            var actions = await _service.GetActions();

            var elements = actions.Select(a => GlobalContext.InitializeEntity(new RemoteDefaultElement(new Identifier($"{Id.Value}_{a.Id}"), a, _service))).Take(LayoutContext.ButtonCount.Width * LayoutContext.ButtonCount.Height - 1);
            
            base.AddElementsInRectangle(elements);
        }
    }
}
