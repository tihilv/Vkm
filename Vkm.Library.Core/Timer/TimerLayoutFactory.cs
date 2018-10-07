using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Library.Timer
{
    public class TimerLayoutFactory: ILayoutFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.TimerLayout.Factory");

        public Identifier Id => Identifier;

        public string Name => "Timers";

        public ILayout CreateLayout(Identifier id)
        {
            return new TimerLayout(id);
        }
    }
}
