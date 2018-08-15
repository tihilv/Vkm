using System;

namespace Vkm.Api.Processes
{
    public class ProcessEventArgs : EventArgs
    {
        public string ProcessName { get; set; }

        public ProcessEventArgs(string processName)
        {
            ProcessName = processName;
        }
    }
}