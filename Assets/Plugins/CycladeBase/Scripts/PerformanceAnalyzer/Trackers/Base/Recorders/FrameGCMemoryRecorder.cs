using Unity.Profiling;

namespace CycladeBase.PerformanceAnalyzer.Trackers.Base.Recorders
{
    public class FrameGCMemoryRecorder : BaseRecorder
    {
        private ProfilerRecorder _recorder;

        public FrameGCMemoryRecorder() : base("Frame GC" , false)
        {
            _recorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
        }

        public override bool UpdateValue()
        {
            FloatValue = _recorder.LastValue / 1024f;
            return true;
        }

        public override void Dispose()
        {
            _recorder.Dispose();   
        }
    }
}