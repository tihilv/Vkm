using System;
using Vkm.Api.Service;

namespace Vkm.Library.Interfaces.Services
{
    public interface ICurrentProcessService: IService
    {
        event EventHandler<ProcessEventArgs> ProcessEnter;
        event EventHandler<ProcessEventArgs> ProcessExit;
    }
}
