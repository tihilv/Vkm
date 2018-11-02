using System;

namespace Vkm.Library.Interfaces.Service
{
    public struct ProcessInfo
    {
        public readonly int Id;
        public readonly string Name;
        public readonly IntPtr Handle;
        public readonly string MainWindowText;
        public readonly IntPtr MainWindowHandle;
        public readonly string ExecutableFileName;

        public ProcessInfo(int id, string executableFileName, string name, IntPtr handle, string mainWindowText, IntPtr mainWindowHandle)
        {
            ExecutableFileName = executableFileName;
            Name = name;
            Handle = handle;
            MainWindowHandle = mainWindowHandle;
            Id = id;
            MainWindowText = mainWindowText;
        }
    }
}