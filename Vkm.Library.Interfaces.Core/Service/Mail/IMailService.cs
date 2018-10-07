using Vkm.Api.Service;

namespace Vkm.Library.Interfaces.Service.Mail
{
    public interface IMailService: IService
    {
        void Activate();

        int? GetUnreadMessageCount();
    }
}
