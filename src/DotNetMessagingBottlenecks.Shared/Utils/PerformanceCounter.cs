using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMessagingBottlenecks.Shared.Utils
{
    internal class PerformanceCounter
    {
        private readonly Dictionary<string, long> _counters = new();
        private readonly Dictionary<string, Stopwatch> _timers = new();
        private readonly object _lock = new();

        public void Increment(string counterName, long value = 1)
        {
            lock (_lock)
            {
                _counters[counterName] = _counters.GetValueOrDefault(counterName, 0) + value;
            }
        }

        public void StartTimer(string timerName)
        {
            lock (_lock)
            {
                if (!_timers.ContainsKey(timerName))
                    _timers[timerName] = new Stopwatch();

                _timers[timerName].Restart();
            }
        }

        public TimeSpan StopTimer(string timerName)
        {
            lock (_lock)
            {
                if (_timers.TryGetValue(timerName, out var timer))
                {
                    timer.Stop();
                    return timer.Elapsed;
                }
                return TimeSpan.Zero;
            }
        }

        public long GetCount(string counterName)
        {
            lock (_lock)
            {
                return _counters.GetValueOrDefault(counterName, 0);
            }
        }

        public Dictionary<string, long> GetAllCounters()
        {
            lock (_lock)
            {
                return new Dictionary<string, long>(_counters);
            }
        }

        public void Reset()
        {
            lock (_lock)
            {
                _counters.Clear();
                _timers.Clear();
            }
        }
    }
}
