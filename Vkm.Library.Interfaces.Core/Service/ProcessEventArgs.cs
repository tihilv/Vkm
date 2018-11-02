using System;

namespace Vkm.Library.Interfaces.Services
{
    public class ProcessEventArgs : EventArgs
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }

        public ProcessEventArgs(int processId, string processName)
        {
            ProcessName = processName;
            ProcessId = processId;
        }
    }
}