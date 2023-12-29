using CycladeBase.PerformanceAnalyzer.Trackers.Base;

using static CycladeUIExample.Performance.ExampleMsTrackerValues;

namespace CycladeUIExample.Performance
{
    public class ExampleMsTrackerValues
    {
        public const int Logic = 0;
    }

    public class ExampleMsTracker : StopwatchTracker
    {
        private static ExampleMsTracker _instance;
        public static ExampleMsTracker I => _instance ??= new ExampleMsTracker();

        private ExampleMsTracker()
        {
            RegisterTrackMs(Logic, "Logic").SetCustomThresholds(8, 16);
        }
    }
}