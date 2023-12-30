using System;

namespace CycladeBase.Utils
{
    public class ClTimer
    {
        public long Time { get; private set; }

        public long EndTime { get; private set; }

        public bool IsEnd => DateTimeOffset.Now.ToUnixTimeMilliseconds() > EndTime;
        public bool IsStartedAndIsEnd => IsEnd && IsStarted;
        
        public bool IsStarted { get; private set; }

        public ClTimer(float seconds, bool start = false)
            : this(TimeSpan.FromSeconds(seconds), start) { }

        public ClTimer(TimeSpan timeSpan, bool start = false)
            : this((long)timeSpan.TotalMilliseconds, start) { }

        private ClTimer(long time, bool start = false) => 
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
            EndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() + Time;
            IsStarted = true;
        }

        public void Stop()
        {
            EndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - 1;
            IsStarted = false;
        }

        public void RewindToEnd()
        {
            EndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - 1;
        }
    }
}