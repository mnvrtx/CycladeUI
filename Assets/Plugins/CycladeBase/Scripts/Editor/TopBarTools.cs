using UnityEditor;

namespace CycladeBaseEditor
{
    public static class TopBarTools
    {
        [MenuItem("Cyclade/Tools/" + nameof(CycladeBase.PerformanceAnalyzer.GetAvailableProfilerStats))]
        public static void GetAvailableProfilerStats()
        {
            CycladeBase.PerformanceAnalyzer.GetAvailableProfilerStats.Run();
        }
    }
}