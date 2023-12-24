using CycladeBase.PerformanceAnalyzer.Trackers.Base;

namespace CycladeBase.PerformanceAnalyzer
{
    public class AnalyzeData
    {
        public float ElapsedResult = float.MinValue;
        public long ElapsedResultLong = long.MinValue;

        public IValueGraph ValueGraph;

        public readonly int Index;
        public readonly string Name;
        public readonly ValSuffix ValSuffix;

        public float MiddleValueThreshold = 10;
        public float LowValueThreshold = 5;
        public bool TopValueIsGood;

        public AnalyzeData(int index, string name, ValSuffix valSuffix)
        {
            Index = index;
            Name = name;
            ValSuffix = valSuffix;
        }

        public void SetCustomThresholds(float middleValueThreshold, float lowValueThreshold, bool topValueIsGood = false)
        {
            MiddleValueThreshold = middleValueThreshold;
            LowValueThreshold = lowValueThreshold;
            TopValueIsGood = topValueIsGood;
        }
    }
}