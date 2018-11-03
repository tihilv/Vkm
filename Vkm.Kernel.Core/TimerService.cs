using System;
using System.Collections.Concurrent;
using System.Timers;
using Vkm.Api.Time;

namespace Vkm.Kernel
{
    public class TimerService : ITimerService
    {
        private readonly ConcurrentDictionary<TimerToken, Timer> _timers;

        public TimerService()
        {
            _timers = new ConcurrentDictionary<TimerToken, Timer>();
        }

        public ITimerToken RegisterTimer(TimeSpan interval, Action action, bool executeOnce = false)
        {
            return new TimerToken(this, interval.TotalMilliseconds, action, executeOnce);
        }


        class TimerToken : ITimerToken
        {
            private readonly double _intervalMs;
            private readonly Action _action;
            private readonly bool _executeOnce;

            private readonly TimerService _timerService;

            public TimerToken(TimerService timerService, double intervalMs, Action action, bool executeOnce)
            {
                _timerService = timerService;
                _intervalMs = intervalMs;
                _action = action;
                _executeOnce = executeOnce;
            }

            public void Start()
            {
                Timer timer = new Timer();
                timer.AutoReset = !_executeOnce;
                timer.Interval = _intervalMs;
                timer.Elapsed += TimerOnElapsed;

                _timerService._timers.TryAdd(this, timer);
                timer.Start();
            }

            private void TimerOnElapsed(object sender, ElapsedEventArgs e)
            {
                _action();

                if (_executeOnce)
                    Stop();
            }

            public void Stop()
            {
                if (_timerService._timers.TryRemove(this, out var timer))
                {
                    timer.Stop();
                    timer.Elapsed -= TimerOnElapsed;
                    timer.Dispose();
                }
            }
        }
    }
}