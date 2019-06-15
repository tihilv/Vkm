using System;

namespace Vkm.Library.Interfaces.Services
{
    public class ProcessEventArgs : EventArgs
    {
        public int ProcessId { get; private set; }
        public string ProcessName { get; private set; }

        public bool IsLeave { get; private set; }

        public ProcessEventArgs(int processId, string processName, bool isLeave)
        {
            ProcessName = processName;
            IsLeave = isLeave;
            ProcessId = processId;
        }
    }
}