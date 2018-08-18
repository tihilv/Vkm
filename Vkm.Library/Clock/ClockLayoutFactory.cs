using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Library.Clock
{
    class ClockLayoutFactory: ILayoutFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.DefaultScreensaver.Factory");

        public Identifier Id => Identifier;

        public string Name => "Time Screen Saver";

        public ILayout CreateLayout(Identifier id)
        {
            return new ClockLayout(id);
        }
    }
}
