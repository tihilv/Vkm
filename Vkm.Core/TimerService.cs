using System;
using System.Collections.Concurrent;
using System.Timers;
using Vkm.Api.Time;

namespace Vkm.Core
{
    public class TimerService : ITimerService
    {
        private readonly ConcurrentDictionary<TimerToken, Timer> _timers;

        public TimerService()
        {
            _timers = new ConcurrentDictionary<TimerToken, Timer>();
        }

        public ITimerToken RegisterTimer(TimeSpan interval, Action action)
        {
            return new TimerToken(this, interval.TotalMilliseconds, action);
        }


        class TimerToken : ITimerToken
        {
            private readonly double _intervalMs;
            private readonly Action _action;

            private readonly TimerService _timerService;

            public TimerToken(TimerService timerService, double intervalMs, Action action)
            {
                _timerService = timerService;
                _intervalMs = intervalMs;
                _action = action;
            }

            public void Start()
            {
                Timer timer = new Timer();
                timer.AutoReset = true;
                timer.Interval = _intervalMs;
                timer.Elapsed += TimerOnElapsed;

                _timerService._timers.TryAdd(this, timer);
                timer.Start();
            }

            private void TimerOnElapsed(object sender, ElapsedEventArgs e)
            {
                _action();
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