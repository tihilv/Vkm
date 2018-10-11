using Vkm.Api.Service;

namespace Vkm.Library.Interfaces.Service
{
    public interface IPowerService: IService
    {
        void DoPowerAction(PowerAction action);
    }
}
