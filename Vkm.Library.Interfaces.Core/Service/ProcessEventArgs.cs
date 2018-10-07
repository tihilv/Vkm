using System;

namespace Vkm.Library.Interfaces.Services
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