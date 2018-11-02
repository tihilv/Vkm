using System.Collections.Generic;
using Vkm.Api.Service;

namespace Vkm.Library.Interfaces.Service
{
    public interface IProcessService: IService
    {
        int Start(string path);
        bool Activate(int id);
        List<ProcessInfo> GetProcessesWithWindows();
        void Stop(int id);
    }
}
