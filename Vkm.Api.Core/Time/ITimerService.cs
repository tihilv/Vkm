using System;

namespace Vkm.Api.Time
{
    public interface ITimerService
    {
        ITimerToken RegisterTimer(TimeSpan interval, Action action);
    }

    public interface ITimerToken
    {
        void Start();
        void Stop();
    }
}
