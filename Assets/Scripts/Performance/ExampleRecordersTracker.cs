using CycladeBase.PerformanceAnalyzer.Trackers.Base;
using CycladeUIExample.Performance.CustomRecorders;

namespace CycladeUIExample.Performance
{
    public class ExampleRecordersTracker : BaseRecordersTracker<ExampleRecordersTracker>
    {
        protected override void Awake()
        {
            RegisterRecorder(new UsedMeshMemoryRecorder(), new ValSuffix((long v) => $"{v:N0} MB"))
                .SetCustomThresholds(20, 10);
        }
    }
}