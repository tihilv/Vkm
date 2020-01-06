using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Vkm.Api.Common;
using Vkm.Api.Identification;
using Vkm.Common.Win32.Win32;
using Vkm.Library.Interfaces.Services;

namespace Vkm.Library.Service
{
    class CurrentProcessService: ICurrentProcessService, IDisposable
    {
        private const string ApplicationFrameHost = "ApplicationFrameHost";

        Win32.WinEventDelegate _delegate = null;
        private IntPtr _hook;
        
        private string _lastProcessName;
        private int? _lastProcessId;

        private readonly ConcurrentQueue<ProcessEventArgs> _notifyArgs;
        private readonly Task _notifyTask;
        private readonly CancellationTokenSource _cts;
        private readonly AsyncAutoResetEvent _processEventAdded;

        public event EventHandler<ProcessEventArgs> ProcessEnter;
        public event EventHandler<ProcessEventArgs> ProcessLeave;
        

        public CurrentProcessService()
        {
            _delegate = new Win32.WinEventDelegate(WinEventProc);
            _hook = Win32.SetWinEventHook(Win32.EVENT_SYSTEM_FOREGROUND, Win32.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _delegate, 0, 0, Win32.WINEVENT_OUTOFCONTEXT);

            _cts = new CancellationTokenSource();
            _processEventAdded = new AsyncAutoResetEvent();
            _notifyArgs = new ConcurrentQueue<ProcessEventArgs>();
            _notifyTask = NotifyCycle(_cts.Token);
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            var process = Process.GetProcessById(GetWindowProcessId(hwnd));
            if (process.ProcessName == ApplicationFrameHost)
                process = GetRealProcess(process);

            var processName = process?.ProcessName;
            var processId = process?.Id;
            if (processId != null)
            {
                if (processId != _lastProcessId)
                {
                    if (_lastProcessId != null)
                        _notifyArgs.Enqueue(new ProcessEventArgs(_lastProcessId.Value, _lastProcessName, true));

                    _lastProcessId = processId;
                    _lastProcessName = processName;
                    _notifyArgs.Enqueue(new ProcessEventArgs(processId.Value, processName, false));
                }

                _processEventAdded.Set();
            }
        }

        async Task NotifyCycle(CancellationToken ctsToken)
        {
            do
            {
                await _processEventAdded.WaitAsync();

                while (_notifyArgs.TryDequeue(out var args))
                {
                    if (args.IsLeave)
                        ProcessLeave?.Invoke(this, args);
                    else
                        ProcessEnter?.Invoke(this, args);
                }


            } while (!ctsToken.IsCancellationRequested);
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

        public void Dispose()
        {
            _cts.Cancel();
        }
    }
}
