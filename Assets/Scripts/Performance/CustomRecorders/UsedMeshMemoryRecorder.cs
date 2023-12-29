using CycladeBase.PerformanceAnalyzer.Trackers.Base.Recorders;
using Unity.Profiling;

namespace CycladeUIExample.Performance.CustomRecorders
{
    public class UsedMeshMemoryRecorder : BaseRecorder
    {
        private ProfilerRecorder _memoryRecorder;

        public UsedMeshMemoryRecorder() : base("Mesh Memory" , true)
        {
            _memoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Mesh Memory");
        }

        public override void UpdateValue()
        {
            var raw = _memoryRecorder.LastValue;
            LongValue = raw / (1024 * 1024); //MB
        }

        public override void Dispose()
        {
            _memoryRecorder.Dispose();   
        }
    }
}