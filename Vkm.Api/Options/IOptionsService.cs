namespace Vkm.Api.Options
{
    public interface IOptionsService
    {
        void InitEntity(IOptionsProvider optionsProvider);
        void SaveOptions();
    }
}