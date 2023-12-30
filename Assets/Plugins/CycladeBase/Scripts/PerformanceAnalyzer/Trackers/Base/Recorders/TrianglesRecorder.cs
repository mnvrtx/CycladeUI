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

        public override bool UpdateValue()
        {
            long count;
#if UNITY_EDITOR
            count = UnityEditor.UnityStats.triangles;
#else
            count = _recorder.LastValue;
#endif

            if (count > 0)
                LongValue = count;
            return true;
        }

        public override void Dispose()
        {
            _recorder.Dispose();   
        }
    }
}