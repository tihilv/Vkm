using System;
using System.Diagnostics;
using Vkm.Api.Identification;
using Vkm.Common.Win32.Win32;
using Vkm.Library.Interfaces.Services;

namespace Vkm.Library.Service
{
    class CurrentProcessService: ICurrentProcessService
    {
        private const string ApplicationFrameHost = "ApplicationFrameHost";

        Win32.WinEventDelegate _delegate = null;
        private IntPtr _hook;


        private string _lastProcess;

        public event EventHandler<ProcessEventArgs> ProcessEnter;
        public event EventHandler<ProcessEventArgs> ProcessExit;

        public CurrentProcessService()
        {
            _delegate = new Win32.WinEventDelegate(WinEventProc);
            _hook = Win32.SetWinEventHook(Win32.EVENT_SYSTEM_FOREGROUND, Win32.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _delegate, 0, 0, Win32.WINEVENT_OUTOFCONTEXT);
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            var process = Process.GetProcessById(GetWindowProcessId(hwnd));
            if (process.ProcessName == ApplicationFrameHost)
                process = GetRealProcess(process);

            var processName = process?.ProcessName;
            if (processName != null)
            {
                if (processName != _lastProcess)
                {
                    if (!string.IsNullOrEmpty(_lastProcess))
                        ProcessExit?.Invoke(this, new ProcessEventArgs(_lastProcess));

                    _lastProcess = processName;
                    ProcessEnter?.Invoke(this, new ProcessEventArgs(processName));
                }
            }
        } 

        private static int GetWindowProcessId(IntPtr hwnd)
        {
            Win32.GetWindowThreadProcessId(hwnd, out var pid);
            return pid;
        }

        private Process _realProcess;

        private Process GetRealProcess(Process foregroundProcess)
        {
            Win32.EnumChildWindows(foregroundProcess.MainWindowHandle, ChildWindowCallback, IntPtr.Zero);
            return _realProcess;
        }

        private bool ChildWindowCallback(IntPtr hwnd, IntPtr lparam)
        {
            var process = Process.GetProcessById(GetWindowProcessId(hwnd));
            if (process.ProcessName != ApplicationFrameHost)
                _realProcess = process;

            return true;
        }

        public Identifier Id => new Identifier("Vkm.CurrentProcessService");
        public string Name => "Current process watcher";
    }
}
