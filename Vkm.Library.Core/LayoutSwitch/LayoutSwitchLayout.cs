using System.Collections.Generic;
using System.Linq;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Library.LayoutSwitch
{
    public class LayoutSwitchLayout: LayoutBase
    {
        public LayoutSwitchLayout(Identifier identifier) : base(identifier)
        {
        }

        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            var elements = new List<LayoutSwitchElement>();

            var layouts = layoutContext.GetActiveLayouts().Where(l => l != this);
            foreach (var layout in layouts)
            {
                var element = GlobalContext.InitializeEntity(new LayoutSwitchElement(new Identifier(Id.Value + ".Elements." + layout.Id.Value)));
                element.SetData(layout, previousLayout);
                elements.Add(element);
            }
            
            AddElementsInRectangle(elements);
        }

        protected override void OnLeavedLayout()
        {
            foreach(var element in Elements.Select(e=>e.Location))
                RemoveElement(element);
        }
    }
}