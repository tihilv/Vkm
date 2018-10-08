using Vkm.Api.Service;

namespace Vkm.Library.Interfaces.Service
{
    public interface ISystemStatusService: IService
    {
        int GetCpuLoad();
        long GetTotalMemoryMBytes();
        long GetFreeMemoryMBytes();

        NetworkStats GetNetworkStats();
    }

    public struct NetworkStats
    {
        public readonly long Sent;
        public readonly long Received;

        public NetworkStats(long sent, long received)
        {
            Sent = sent;
            Received = received;
        }
    }
}
