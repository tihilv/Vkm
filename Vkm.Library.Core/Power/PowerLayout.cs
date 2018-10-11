using Vkm.Api.Basic;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Library.Common;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.Power
{
    class PowerLayout: LayoutBase
    {
        public PowerLayout(Identifier identifier) : base(identifier)
        {
        }

        public override void Init()
        {
            base.Init();

            IElement element = new PowerElement(new Identifier(Id.Value + ".Logoff"), PowerAction.LogOff);
            AddElement(new Location(1,1), GlobalContext.InitializeEntity(element));
            
            element = new PowerElement(new Identifier(Id.Value + ".Reboot"), PowerAction.Reboot);
            AddElement(new Location(2,1), GlobalContext.InitializeEntity(element));

            element = new PowerElement(new Identifier(Id.Value + ".PowerOff"), PowerAction.PowerOff);
            AddElement(new Location(3,1), GlobalContext.InitializeEntity(element));

            element = new BackElement();
            AddElement(new Location(4,2), GlobalContext.InitializeEntity(element));

        }
    }
}
