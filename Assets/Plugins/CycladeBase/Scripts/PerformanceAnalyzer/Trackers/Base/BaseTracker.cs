using System.Collections.Generic;

namespace CycladeBase.PerformanceAnalyzer.Trackers.Base
{
    public class BaseTracker : ITrackerResultsProvider
    {
        public IReadOnlyDictionary<int, AnalyzeData> Results => _elapsedResults;

        private readonly Dictionary<int, AnalyzeData> _elapsedResults = new();

        public AnalyzeData RegisterTrackMs(int metric, string name) => RegisterTrack(metric, name, new ValSuffix((float f) => $"{f:F2}MS"));

        public AnalyzeData RegisterTrack(int metric, string name, ValSuffix valSuffix = null)
        {
            if (valSuffix == null)
                valSuffix = new ValSuffix((float val) => $"{val:F2}");
            var analyzeData = new AnalyzeData(metric, name, valSuffix);
            _elapsedResults[metric] = analyzeData;
            return analyzeData;
        }

        public void FinishTrackInternal(int idx, float value)
        {
            var data = _elapsedResults[idx];
            data.ElapsedResult = value;
            data.ValueGraph?.Update();
        }

        public void FinishTrackInternalLong(int idx, long value)
        {
            var data = _elapsedResults[idx];
            data.ElapsedResultLong = value;
            data.ValueGraph?.Update();
        }

        public void Clear()
        {
            _elapsedResults.Clear();
        }
    }
}