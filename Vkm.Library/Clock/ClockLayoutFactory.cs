using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;

namespace Vkm.Library.Clock
{
    class ClockLayoutFactory: ILayoutFactory
    {
        public Identifier Id => Identifiers.DefaultScreenSaverFactory;

        public string Name => "Time Screen Saver";

        public ILayout CreateLayout(Identifier id)
        {
            return new ClockLayout(id);
        }
    }
}
