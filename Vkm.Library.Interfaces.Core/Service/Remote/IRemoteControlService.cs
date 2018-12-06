using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vkm.Api.Service;

namespace Vkm.Library.Interfaces.Service.Remote
{
    public interface IRemoteControlService: IService
    {
        Task<List<ActionInfo>> GetActions();

        void StartAction(ActionInfo action);

        event EventHandler<ActionEventArgs> ActiveActionChanged;
    }
}