namespace Vkm.Intercom.Dispatchers
{
    public interface IDispatcherService : IRemoteService
    {
        string Dispatch();
    }
}