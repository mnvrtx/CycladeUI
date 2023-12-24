using System.Collections.Generic;

namespace CycladeBase.PerformanceAnalyzer.Trackers.Base
{
    public interface ITrackerResultsProvider
    {
        public IReadOnlyDictionary<int, AnalyzeData> Results { get; }
    }
}