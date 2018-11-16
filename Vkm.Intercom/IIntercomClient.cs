namespace Vkm.Intercom
{
    public interface IIntercomClient
    {
        object Execute(string method, params object[] arguments);
    }
}