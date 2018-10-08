using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Common.Win32.Win32;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.Service
{
    class SystemStatusService: ISystemStatusService, IInitializable
    {
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _ramCounter;
        private long _totalMemory;

        private NetworkInterface[] _networkInterfaces;
        private IPv4InterfaceStatistics[] _netStats;

        public Identifier Id => new Identifier("Vkm.SystemStatusService");
        public string Name => "System Status Service";


        public void InitContext(GlobalContext context)
        {
            
        }

        public void Init()
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            _networkInterfaces = NetworkInterface.GetAllNetworkInterfaces().Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback).ToArray();
            _netStats = new IPv4InterfaceStatistics[_networkInterfaces.Length];

            Win32.GetPhysicallyInstalledSystemMemory(out _totalMemory);
            _totalMemory /= 1024;
        }

        public int GetCpuLoad()
        {
            return (int) _cpuCounter.NextValue();
        }

        public long GetTotalMemoryMBytes()
        {
            return _totalMemory;
        }

        public long GetFreeMemoryMBytes()
        {
            return (long)_ramCounter.NextValue();
        }

        public NetworkStats GetNetworkStats()
        {
            for (int i = 0; i < _networkInterfaces.Length; i++)
                _netStats[i] = _networkInterfaces[i].GetIPv4Statistics();

            var sent = _netStats.Sum(n => n.BytesSent);
            var recv = _netStats.Sum(n => n.BytesReceived);

            return new NetworkStats(sent, recv);
        }
    }
}
