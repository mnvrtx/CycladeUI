using System;
using BaseShared;

namespace Shared.Utils
{
    public class Timer
    {
        public long Time { get; private set; }

        public long EndTime { get; private set; }

        public bool IsEnd => GeneralTime.NowMs > EndTime;
        public bool IsStartedAndIsEnd => IsEnd && IsStarted;
        
        public bool IsStarted { get; private set; }

        public Timer(float seconds, bool start = false)
            : this(TimeSpan.FromSeconds(seconds), start) { }

        public Timer(TimeSpan timeSpan, bool start = false)
            : this((long)timeSpan.TotalMilliseconds, start) { }

        private Timer(long time, bool start = false) => 
            Set(time, start);

        public void Set(long time, bool start = false)
        {
            Time = time;
            if (start)
                Start();
        }

        public void Restart() => Start();
        public void Start()
        {
            EndTime = GeneralTime.NowMs + Time;
            IsStarted = true;
        }

        public void Stop()
        {
            EndTime = GeneralTime.NowMs - 1;
            IsStarted = false;
        }

        public void RewindToEnd()
        {
            EndTime = GeneralTime.NowMs - 1;
        }
    }
}