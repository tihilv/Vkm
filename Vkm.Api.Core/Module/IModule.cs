using Vkm.Api.Identification;

namespace Vkm.Api.Module
{
    public interface IModule: IIdentifiable
    {
        string Name { get; }
    }
}