using Unity.Profiling;

namespace CycladeBase.PerformanceAnalyzer.Trackers.Base.Recorders
{
    public class UsedMemoryRecorder : BaseRecorder
    {
        private ProfilerRecorder _usedMemoryRecorder;

        public UsedMemoryRecorder() : base("Used Memory" , true)
        {
            _usedMemoryRecorder =
#if (UNITY_IOS && !UNITY_EDITOR)
                ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
#elif (UNITY_ANDROID && !UNITY_EDITOR)
                ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");
#else
                ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
#endif
        }

        public override bool UpdateValue()
        {
            var raw = _usedMemoryRecorder.LastValue;
            LongValue = raw / (1024 * 1024); //MB
            return true;
        }

        public override void Dispose()
        {
            _usedMemoryRecorder.Dispose();   
        }
    }
}