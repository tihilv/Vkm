using System;
using Vkm.Api.Time;

namespace Vkm.TestProject.Entities
{
    internal class TestTimerService: ITimerService
    {
        public ITimerToken RegisterTimer(TimeSpan interval, Action action, bool executeOnce = false)
        {
            return null;
        }
    }
}