﻿using System;
using System.Runtime.InteropServices;

using CoreAudioApi.Interfaces;

namespace CoreAudioApi
{
    /// <summary>
    /// AudioClock is used to get the current device position
    /// <remarks>obtains from AudioClient</remarks>
    /// </summary>
    public class AudioClock : IDisposable
    {
        private IAudioClock2 _RealClock;

        internal AudioClock(IAudioClock2 realClock)
        {
            _RealClock = realClock;
        }

        public void Dispose()
        {
            if (_RealClock != null)
            {
                Marshal.ReleaseComObject(_RealClock);
            }
        }

        /// <summary>
        /// The device frequency is the frequency generated by the hardware clock in the audio device. This method reports the device frequency in units that are compatible with those of the device position that the IAudioClock::GetPosition method reports.
        /// </summary>
        public UInt64 GetFrequency()
        {
            UInt64 freq;
            int hr = _RealClock.GetFrequency(out freq);
            Marshal.ThrowExceptionForHR(hr);
            return freq;
        }

        /// <summary>
        /// The device position is the offset from the start of the stream to the current position in the stream. However, the units in which this offset is expressed are undefined—the device position value has meaning only in relation to the frequency reported by the IAudioClock::GetFrequency method.
        /// </summary>
        public UInt64 GetPosition()
        {
            UInt64 pos; UInt64 qpcPos;
            int hr = _RealClock.GetPosition(out pos, out qpcPos);
            Marshal.ThrowExceptionForHR(hr);
            return pos;
        }

        /// <summary>
        /// gets the current device position
        /// </summary>
        /// <param name="position">The device position is the offset from the start of the stream to the current position in the stream. However, the units in which this offset is expressed are undefined—the device position value has meaning only in relation to the frequency reported by the IAudioClock::GetFrequency method.</param>
        /// <param name="qpcPosition">Pointer to a UINT64 variable into which the method writes the value of the performance counter at the time that the audio endpoint device read the device position (*pu64Position) in response to the GetPosition call. The method converts the counter value to 100-nanosecond time units before writing it to *pu64QPCPosition.</param>
        public void GetPosition(out UInt64 position, out UInt64 qpcPosition)
        {
            int hr = _RealClock.GetPosition(out position, out qpcPosition);
            Marshal.ThrowExceptionForHR(hr);
        }

        /*
        /// <summary>
        /// reserved for future use.
        /// </summary>
        /// <returns></returns>
        public uint GetCharacteristics()
        {
            uint characteristics;
            int hr = _RealClock.GetCharacteristics(out characteristics);
            Marshal.ThrowExceptionForHR(hr);
            return characteristics;
        }
         */

        /// <summary>
        /// Receives the device position, in frames. The received position is an unprocessed value that the method obtains directly from the hardware.
        /// </summary>
        public UInt64 GetDevicePosition()
        {
            UInt64 pos; UInt64 qpcPos;
            int hr = _RealClock.GetDevicePosition(out pos, out qpcPos);
            Marshal.ThrowExceptionForHR(hr);
            return pos;
        }

        /// <summary>
        /// gets the current device position, in frames, directly from the hardware
        /// </summary>
        /// <param name="devicePosition">Receives the device position, in frames. The received position is an unprocessed value that the method obtains directly from the hardware.</param>
        /// <param name="qpcPosition">Receives the value of the performance counter at the time that the audio endpoint device read the device position retrieved in the DevicePosition parameter in response to the GetDevicePosition call. GetDevicePosition converts the counter value to 100-nanosecond time units before writing it to QPCPosition. QPCPosition can be NULL if the client does not require the performance counter value.</param>
        public void GetDevicePosition(out UInt64 devicePosition, out UInt64 qpcPosition)
        {
            int hr = _RealClock.GetDevicePosition(out devicePosition, out qpcPosition);
            Marshal.ThrowExceptionForHR(hr);
        }
    }

    /// <summary>
    /// AudioClockAdjustment adjusts the sample rate of a stream
    /// <remarks>The IAudioClockAdjustment interface must be obtained from an audio client that is initialized with both the AUDCLNT_STREAMFLAGS_RATEADJUST flag and the share mode set to AUDCLNT_SHAREMODE_SHARED. If Initialize is called in an exclusive mode with the AUDCLNT_STREAMFLAGS_RATEADJUST flag, Initialize fails with the AUDCLNT_E_UNSUPPORTED_FORMAT error code.</remarks>
    /// </summary>
    public class AudioClockAdjustment : IDisposable
    {
        private IAudioClockAdjustment _realAdj;

        internal AudioClockAdjustment(IAudioClockAdjustment realAdj)
        {
            _realAdj = realAdj;
        }
        public void Dispose()
        {
            if (_realAdj != null)
            {
                Marshal.ReleaseComObject(_realAdj);
            }
        }

        public void SetSampleRate(float sampleRate)
        {
            int hr = _realAdj.SetSampleRate(sampleRate);
            Marshal.ThrowExceptionForHR(hr);
        }

    }
}
