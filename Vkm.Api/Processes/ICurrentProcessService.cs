using System;

namespace Vkm.Api.Processes
{
    public interface ICurrentProcessService
    {
        event EventHandler<ProcessEventArgs> ProcessEnter;
        event EventHandler<ProcessEventArgs> ProcessExit;
    }
}
