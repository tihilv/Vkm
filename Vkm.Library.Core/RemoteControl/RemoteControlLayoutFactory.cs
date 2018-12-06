using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Library.RemoteControl
{
    public class RemoteControlLayoutFactory: ILayoutFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.DefaultRemoteControl.Factory");

        public Identifier Id => Identifier;

        public string Name => "Remote Controls";

        public ILayout CreateLayout(Identifier id)
        {
            return new RemoteControlLayout(id);
        }
    }
}
