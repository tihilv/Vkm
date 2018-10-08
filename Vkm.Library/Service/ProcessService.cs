using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Vkm.Api.Identification;
using Vkm.Common.Win32.Win32;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.Service
{
    public class ProcessService: IProcessService
    {
        private readonly ConcurrentDictionary<IntPtr, Process> _processes;

        public ProcessService()
        {
            _processes = new ConcurrentDictionary<IntPtr, Process>();
        }

        public Identifier Id => new Identifier("Vkm.ProcessService");
        public string Name => "Process Service";

        public IntPtr Start(string path)
        {
            var process = Process.Start(path);
            if (process != null)
            {
                _processes[process.MainWindowHandle] = process;
                return process.MainWindowHandle;
            }
            return IntPtr.Zero;
        }

        public bool Activate(IntPtr handle)
        {
            if (!_processes.TryGetValue(handle, out var process) || process.HasExited)
                return false;

            Win32.SwitchToThisWindow(handle, true);
            return true;
        }
    }
}
