using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Principal;
using Vkm.Api.Identification;
using Vkm.Common.Win32.Win32;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.Service
{
    public class ProcessService: IProcessService
    {
        public Identifier Id => new Identifier("Vkm.ProcessService");
        public string Name => "Process Service";

        public int Start(string path)
        {
            var process = Process.Start(path);
            if (process != null)
            {
                return process.Id;
            }
            return 0;
        }

        public bool Activate(int id)
        {
            try
            {
                var process = Process.GetProcessById(id);
                if (process.HasExited)
                    return false;

                Win32.SwitchToThisWindow(process.MainWindowHandle, true);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public List<ProcessInfo> GetProcessesWithWindows()
        {
            List<ProcessInfo> result = new List<ProcessInfo>();

            var currentSessionId = Process.GetCurrentProcess().SessionId;

            foreach (var item in Process.GetProcesses())
                if (item.SessionId == currentSessionId)
                {
                    try
                    {
                        if (item.MainWindowTitle.Length > 0)
                        {
                            string executable = GetCommandLine(item.Id);
                            if (File.Exists(executable))
                            {
                                result.Add(new ProcessInfo(item.Id, executable, item.ProcessName, item.Handle, item.MainWindowTitle, item.MainWindowHandle));
                            }
                        }
                    }
                    catch (InvalidOperationException)
                    {

                    }
                }

            return result;
        }

        public void Stop(int id)
        {
            try
            {
                Process.GetProcessById(id).CloseMainWindow();
            }
            catch (ArgumentException)
            {

            }
        }

        private Dictionary<int, string> _cachedCommandLines;
        private string GetCommandLine(int processId)
        {
            string result;

            if (_cachedCommandLines == null || !_cachedCommandLines.TryGetValue(processId, out result))
            {
                _cachedCommandLines = GetCommandLines();
                _cachedCommandLines.TryGetValue(processId, out result);
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
