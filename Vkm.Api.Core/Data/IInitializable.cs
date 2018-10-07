namespace Vkm.Api.Data
{
    public interface IInitializable
    {
        void InitContext(GlobalContext context);
        void Init();
    }
}