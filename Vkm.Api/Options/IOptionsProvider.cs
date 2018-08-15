using Vkm.Api.Identification;

namespace Vkm.Api.Options
{
    public interface IOptionsProvider: IIdentifiable
    {
        IOptions GetDefaultOptions();

        void InitOptions(IOptions options);
    }
}