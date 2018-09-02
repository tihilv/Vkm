using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Vkm.Api
{
    public static class Win32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        public static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);        

        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        public const uint WINEVENT_OUTOFCONTEXT = 0;
        public const uint EVENT_SYSTEM_FOREGROUND = 3;

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        public delegate bool WindowEnumProc(IntPtr hwnd, IntPtr lparam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc callback, IntPtr lParam);

        [DllImport("user32.dll")]   
        public static extern void SwitchToThisWindow(IntPtr hWnd,bool turnon);

        [StructLayout( LayoutKind.Sequential )]
        public struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;    
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);
        public const byte VK_MEDIA_NEXT_TRACK = 0xB0;
        public const byte VK_MEDIA_PLAY_PAUSE = 0xB3;
        public const byte VK_MEDIA_PREV_TRACK = 0xB1;
        
        public const byte KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const byte KEYEVENTF_KEYUP = 0x0002; //Key up flag

        public static void ButtonPress(byte button)
        {
            keybd_event(button, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
            keybd_event(button, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        internal struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        [DllImport("kernel32.dll", ExactSpelling=true) ]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling=true, SetLastError=true) ]
        internal static extern bool OpenProcessToken( IntPtr h, int acc, ref IntPtr
            phtok );

        [DllImport("advapi32.dll", SetLastError=true) ]
        internal static extern bool LookupPrivilegeValue( string host, string name,
            ref long pluid );

        [DllImport("advapi32.dll", ExactSpelling=true, SetLastError=true) ]
        internal static extern bool AdjustTokenPrivileges( IntPtr htok, bool disall,
            ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen );

        [DllImport("user32.dll", ExactSpelling=true, SetLastError=true) ]
        internal static extern bool ExitWindowsEx( int flg, int rea );

        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        internal const int EWX_FORCE = 0x00000004;
        internal const int EWX_POWEROFF = 0x00000008;
        internal const int EWX_FORCEIFHUNG = 0x00000010;

        public static void DoExitWin( int flg )
        {
            bool ok;
            TokPriv1Luid tp;
            IntPtr hproc = GetCurrentProcess();
            IntPtr htok = IntPtr.Zero;
            ok = OpenProcessToken( hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok );
            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = SE_PRIVILEGE_ENABLED;
            ok = LookupPrivilegeValue( null, SE_SHUTDOWN_NAME, ref tp.Luid );
            ok = AdjustTokenPrivileges( htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero );
            ok = ExitWindowsEx( flg, 0 );
        }

    }
}
