using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
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

        public List<ProcessInfo> GetProcessesWithWindows()
        {
            List<ProcessInfo> result = new List<ProcessInfo>();

            var commandLines = GetCommandLines();

            foreach (var item in Process.GetProcesses())
            {
                try
                {
                    if (item.MainWindowTitle.Length > 0)
                    {
                        commandLines.TryGetValue(item.Id, out var executable);
                        if (File.Exists(executable))
                        {
                            result.Add(new ProcessInfo(executable, item.ProcessName, item.Handle, item.MainWindowTitle, item.MainWindowHandle));
                            _processes[item.MainWindowHandle] = item;
                        }
                    }
                }
                catch (InvalidOperationException)
                {

                }
            }

            return result;
        }

        private Dictionary<int, string> GetCommandLines()
        {
            Dictionary<int, string> result = new Dictionary<int, string>();

            var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
            {
                foreach (var obj in results.Cast<ManagementObject>())
                {
                    result.Add((int) (uint) obj["ProcessId"], (string) obj["ExecutablePath"]);
                }
            }

            return result;
        }
    }
}
