using CycladeBase.PerformanceAnalyzer.Trackers.Base;

using static CycladeUIExample.Performance.ExampleValueTrackerValues;

namespace CycladeUIExample.Performance
{
    public class ExampleValueTrackerValues
    {
        public const int Tracker = 0;
    }

    public class ExampleValueTracker : BaseTracker
    {
        private static ExampleValueTracker _instance;
        public static ExampleValueTracker I => _instance ??= new ExampleValueTracker();

        private ExampleValueTracker()
        {
            RegisterTrack(Tracker, "Tracker").SetCustomThresholds(30.2f, 15.5f);
        }
    }
}