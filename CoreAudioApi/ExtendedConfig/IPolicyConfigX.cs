using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CoreAudioApi.ExtendedConfig
{
    [Guid("00000000-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPolicyConfigX
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetMixFormat([MarshalAs(UnmanagedType.LPWStr), In] string pszDeviceName, out WAVEFORMATEXTENSIBLE ppFormat);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetDeviceFormat([MarshalAs(UnmanagedType.LPWStr), In] string pszDeviceName, [MarshalAs(UnmanagedType.Bool), In] bool bDefault, out WAVEFORMATEXTENSIBLE ppFormat);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int ResetDeviceFormat([MarshalAs(UnmanagedType.LPWStr), In] string pszDeviceName);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int SetDeviceFormat([MarshalAs(UnmanagedType.LPWStr), In] string pszDeviceName, [In] WAVEFORMATEXTENSIBLE pEndpointFormat, [In] WAVEFORMATEXTENSIBLE mixFormat);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetProcessingPeriod([MarshalAs(UnmanagedType.LPWStr), In] string pszDeviceName, [MarshalAs(UnmanagedType.Bool), In] bool bDefault, [In] IntPtr pmftDefaultPeriod, [In] IntPtr pmftMinimumPeriod);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int SetProcessingPeriod([MarshalAs(UnmanagedType.LPWStr), In] string pszDeviceName, [In] IntPtr pmftPeriod);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetShareMode([MarshalAs(UnmanagedType.LPWStr), In] string pszDeviceName, out AudioClientShareMode pMode);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int SetShareMode([MarshalAs(UnmanagedType.LPWStr), In] string pszDeviceName, [In] AudioClientShareMode mode);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int GetPropertyValue([MarshalAs(UnmanagedType.LPWStr), In] string pszDeviceName, [MarshalAs(UnmanagedType.Bool), In] bool bFxStore, [In] IntPtr key, [In] IntPtr pv);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int SetPropertyValue([MarshalAs(UnmanagedType.LPWStr), In] string pszDeviceName, [MarshalAs(UnmanagedType.Bool), In] bool bFxStore, [In] IntPtr key, [In] IntPtr pv);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int SetDefaultEndpoint([MarshalAs(UnmanagedType.LPWStr), In] string pszDeviceName, [MarshalAs(UnmanagedType.U4), In] ERole role);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int SetEndpointVisibility([MarshalAs(UnmanagedType.LPWStr), In] string pszDeviceName, [MarshalAs(UnmanagedType.Bool), In] bool bVisible);
    }
}