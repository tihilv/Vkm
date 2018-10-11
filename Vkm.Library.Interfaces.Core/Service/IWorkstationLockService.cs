using System;
using Vkm.Api.Service;

namespace Vkm.Library.Interfaces.Service
{
    public interface IWorkstationLockService: IService
    {
        bool Locked { get; }
        event EventHandler<LockEventArgs> LockChanged;
        TimeSpan GetIdleTimeSpan();
    }
}