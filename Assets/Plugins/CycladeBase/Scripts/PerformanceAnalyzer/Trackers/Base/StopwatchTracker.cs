using System;
using System.Collections.Generic;
using System.Diagnostics;
using CycladeBase.Utils;

namespace CycladeBase.PerformanceAnalyzer.Trackers.Base
{
    public abstract class StopwatchTracker : BaseTracker
    {
        private readonly List<Stopwatch> _stopwatches = new();

        public void BeginTrack(int metric)
        {
            if (!_stopwatches.ValidIndex(metric))
            {
                if (metric < 0)
                    throw new Exception("metric < 0");
                _stopwatches.Set(metric, Stopwatch.StartNew());
                return;
            }
            
            _stopwatches[metric].Restart();
        }

        public void FinishTrack(int metric)
        {
            CommitTrack(metric, _stopwatches[metric].ElapsedMilliseconds);
        }
    }
}