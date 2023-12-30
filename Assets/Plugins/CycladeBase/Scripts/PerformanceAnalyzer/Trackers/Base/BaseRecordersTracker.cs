using System.Collections.Generic;
using CycladeBase.PerformanceAnalyzer.Trackers.Base.Recorders;
using CycladeBase.Utils;

namespace CycladeBase.PerformanceAnalyzer.Trackers.Base
{
    public abstract class BaseRecordersTracker<T> : MonoBehaviourSingleton<T>, ITrackerResultsProvider where T : MonoBehaviourSingleton<T>
    {
        private readonly List<BaseRecorder> _recorders = new();

        public IReadOnlyDictionary<int, AnalyzeData> Results => _tracker.Results;

        private readonly BaseTracker _tracker = new();

        protected abstract void Awake();

        protected AnalyzeData RegisterRecorder(BaseRecorder recorder, ValSuffix valSuffix = null, string viewTitle = null)
        {
            var analyzeData = _tracker.RegisterTrack(_recorders.Count, !string.IsNullOrEmpty(viewTitle) ? viewTitle : recorder.Name, valSuffix);
            _recorders.Add(recorder);
            return analyzeData;
        }

        public void LateUpdate()
        {
            for (var i = 0; i < _recorders.Count; i++)
            {
                var recorder = _recorders[i];
                if (recorder.UpdateValue())
                {
                    if (recorder.IsLongValue)
                        _tracker.CommitTrackLong(i, recorder.LongValue);
                    else
                        _tracker.CommitTrack(i, recorder.FloatValue);    
                }
                
            }
        }

        private void OnDestroy()
        {
            foreach (var recorder in _recorders)
                recorder.Dispose();
            _tracker.Clear();
        }
    }
}