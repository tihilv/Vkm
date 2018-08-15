using Vkm.Api.Basic;
using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Library.Numpad
{
    internal class NumpadLayout: LayoutBase
    {
        public NumpadLayout(Identifier id): base(id)
        {
        }

        public override void Init()
        {
            base.Init();

            AddElement(new Location(0,0), GlobalContext.InitializeEntity(new ButtonElement("7")));
            AddElement(new Location(1,0), GlobalContext.InitializeEntity(new ButtonElement("8")));
            AddElement(new Location(2,0), GlobalContext.InitializeEntity(new ButtonElement("9")));

            AddElement(new Location(0,1), GlobalContext.InitializeEntity(new ButtonElement("4")));
            AddElement(new Location(1,1), GlobalContext.InitializeEntity(new ButtonElement("5")));
            AddElement(new Location(2,1), GlobalContext.InitializeEntity(new ButtonElement("6")));

            AddElement(new Location(0,2), GlobalContext.InitializeEntity(new ButtonElement("1")));
            AddElement(new Location(1,2), GlobalContext.InitializeEntity(new ButtonElement("2")));
            AddElement(new Location(2,2), GlobalContext.InitializeEntity(new ButtonElement("3")));

            AddElement(new Location(3,2), GlobalContext.InitializeEntity(new ButtonElement("0")));
            AddElement(new Location(4,2), GlobalContext.InitializeEntity(new ButtonElement("=")));

            AddElement(new Location(3,0), GlobalContext.InitializeEntity(new ButtonElement("*", "{MULTIPLY}")));
            AddElement(new Location(4,0), GlobalContext.InitializeEntity(new ButtonElement("/", "{DIVIDE}")));
            AddElement(new Location(3,1), GlobalContext.InitializeEntity(new ButtonElement("+", "{ADD}")));
            AddElement(new Location(4,1), GlobalContext.InitializeEntity(new ButtonElement("-", "{SUBTRACT}")));

        }
    }
}
