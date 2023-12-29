using Unity.Profiling;

namespace CycladeBase.PerformanceAnalyzer.Trackers.Base.Recorders
{
    public class MainThreadRecorder : BaseRecorder
    {
        private ProfilerRecorder _recorder;

        public MainThreadRecorder() : base("MainThread", false)
        {
            _recorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
        }

        public override void UpdateValue()
        {
            FloatValue = (float)(GetRecorderFrameAverage(_recorder) * 1e-6);
        }
        
        private static double GetRecorderFrameAverage(ProfilerRecorder recorder)
        {
            var samplesCount = recorder.Capacity;
            if (samplesCount == 0)
                return 0;

            double r = 0;
            unsafe
            {
                var samples = stackalloc ProfilerRecorderSample[samplesCount];
                recorder.CopyTo(samples, samplesCount);
                for (var i = 0; i < samplesCount; ++i)
                    r += samples[i].Value;
                r /= samplesCount;
            }

            return r;
        }

        public override void Dispose()
        {
            _recorder.Dispose();   
        }
    }
}