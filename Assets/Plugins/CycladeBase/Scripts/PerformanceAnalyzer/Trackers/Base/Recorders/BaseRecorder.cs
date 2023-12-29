using System;

namespace CycladeBase.PerformanceAnalyzer.Trackers.Base.Recorders
{
    public abstract class BaseRecorder : IDisposable
    {
        public float FloatValue;
        public long LongValue;

        public readonly bool IsLongValue;
        public readonly string Name;
        protected BaseRecorder(string name, bool isLongValue)
        {
            Name = name;
            IsLongValue = isLongValue;
        }

        public abstract void UpdateValue();

        public abstract void Dispose();
    }
}