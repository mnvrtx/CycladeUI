using Unity.Profiling;
using UnityEngine;

namespace CycladeBase.PerformanceAnalyzer.Trackers.Base.Recorders
{
    public class MainThreadRecorder : BaseRecorder
    {
        private ProfilerRecorder _recorder;
        private ProfilerRecorder _vSyncRecorder;

        public MainThreadRecorder() : base("MainThread", false)
        {
            _recorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
            _vSyncRecorder = ProfilerRecorder.StartNew(new ProfilerCategory("VSync"), "TimeUpdate.WaitForLastPresentationAndUpdateTime", 15);
        }

        public override bool UpdateValue()
        {
            // FloatValue = (float)(GetRecorderFrameAverage(_recorder, _vSyncRecorder) * 1e-6);
            FloatValue = Mathf.Max(0, (float)((_recorder.LastValue - _vSyncRecorder.LastValue) * 1e-6));
            return true;
        }
        
        private static double GetRecorderFrameAverage(ProfilerRecorder recorder, ProfilerRecorder recorderExclude)
        {
            var samplesCount = recorder.Capacity;
            if (samplesCount == 0)
                return 0;

            double r = 0;
            unsafe
            {
                var samples = stackalloc ProfilerRecorderSample[samplesCount];
                var excludeSamples = stackalloc ProfilerRecorderSample[samplesCount];
                recorder.CopyTo(samples, samplesCount);
                recorderExclude.CopyTo(excludeSamples, samplesCount);
                for (var i = 0; i < samplesCount; ++i)
                    r += samples[i].Value - excludeSamples[i].Value;
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