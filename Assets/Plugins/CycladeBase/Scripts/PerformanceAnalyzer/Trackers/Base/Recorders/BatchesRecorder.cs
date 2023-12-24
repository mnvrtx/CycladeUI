using Unity.Profiling;

namespace CycladeBase.PerformanceAnalyzer.Trackers.Base.Recorders
{
    public class BatchesRecorder : BaseRecorder
    {
        private ProfilerRecorder _recorder;

        public BatchesRecorder() : base("BatchesCount", true)
        {
            _recorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count");
        }

        public override void UpdateValue()
        {
            long batchesCount;
#if UNITY_EDITOR
            batchesCount = UnityEditor.UnityStats.batches;
#else
            batchesCount = _recorder.LastValue;
#endif

            if (batchesCount > 0)
                LongValue = batchesCount;
        }

        public override void Dispose()
        {
            _recorder.Dispose();   
        }
    }
}