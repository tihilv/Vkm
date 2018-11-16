namespace Vkm.Intercom
{
    public interface IRemoteService
    {
        
    }

    public interface IRemoteServiceWithCallback : IRemoteService
    {
        void RegisterChannel(IIntercomClient client);
    }
}