using System;
using Vkm.Api.Service;

namespace Vkm.Library.Interfaces.Service
{
    public interface IProcessService: IService
    {
        IntPtr Start(string path);
        bool Activate(IntPtr handle);
    }
}
