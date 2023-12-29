using Unity.Profiling;

namespace CycladeBase.PerformanceAnalyzer.Trackers.Base.Recorders
{
    public class TrianglesRecorder : BaseRecorder
    {
        private ProfilerRecorder _recorder;

        public TrianglesRecorder() : base("TrianglesCount", true)
        {
            _recorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
        }

        public override void UpdateValue()
        {
            long count;
#if UNITY_EDITOR
            count = UnityEditor.UnityStats.triangles;
#else
            count = _recorder.LastValue;
#endif

            if (count > 0)
                LongValue = count;
        }

        public override void Dispose()
        {
            _recorder.Dispose();   
        }
    }
}