using System.Collections.Generic;
using CycladeBase.PerformanceAnalyzer.Trackers.Base.Recorders;
using CycladeBase.Utils;

namespace CycladeBase.PerformanceAnalyzer.Trackers.Base
{
    public abstract class BaseRecordersTracker<T> : MonoBehaviourSingleton<T> where T : MonoBehaviourSingleton<T>
    {
        private readonly List<BaseRecorder> _recorders = new();

        public readonly BaseTracker Tracker = new();

        protected abstract void Awake();

        public AnalyzeData RegisterRecorder(BaseRecorder recorder, ValSuffix valSuffix = null, string viewTitle = null)
        {
            var analyzeData = Tracker.RegisterTrack(_recorders.Count, !string.IsNullOrEmpty(viewTitle) ? viewTitle : recorder.Name, valSuffix);
            _recorders.Add(recorder);
            return analyzeData;
        }

        public void LateUpdate()
        {
            for (var i = 0; i < _recorders.Count; i++)
            {
                var recorder = _recorders[i];
                recorder.UpdateValue();
                if (recorder.IsLongValue)
                    Tracker.FinishTrackInternalLong(i, recorder.LongValue);
                else
                    Tracker.FinishTrackInternal(i, recorder.FloatValue);
            }
        }

        private void OnDestroy()
        {
            foreach (var recorder in _recorders)
                recorder.Dispose();
            Tracker.Clear();
        }
    }
}