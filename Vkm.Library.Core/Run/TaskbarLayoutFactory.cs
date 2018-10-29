using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Library.Run
{
    public class TaskbarLayoutFactory : ILayoutFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.TaskbarLayout.Factory");

        public Identifier Id => Identifier;

        public string Name => "Taskbar Layouts";

        public ILayout CreateLayout(Identifier id)
        {
            return new TaskbarLayout(id);
        }
    }
}