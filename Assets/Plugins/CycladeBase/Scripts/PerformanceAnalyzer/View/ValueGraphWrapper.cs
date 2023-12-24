using CycladeBase.PerformanceAnalyzer.Trackers.Base;
using CycladeBase.Utils;

namespace CycladeBase.PerformanceAnalyzer.View
{
    public class ValueGraphWrapper : IValueGraph
    {
        private readonly ValueGraph _valueGraph;
        private readonly AnalyzeData _data;

        public ValueGraphWrapper(ValueGraph valueGraph, AnalyzeData data)
        {
            _valueGraph = valueGraph;
            _data = data;
        }

        public void Update()
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (!_valueGraph.InternalActive && (_data.ElapsedResult != float.MinValue || _data.ElapsedResultLong != long.MinValue))
            {
                _valueGraph.SetActive(true);
                _valueGraph.InternalActive = true;
            }

            if (_data.ValSuffix.IsLong)
                _valueGraph.ProcessLong(_data.ElapsedResultLong);
            else
                _valueGraph.Process(_data.ElapsedResult);
        }
    }
}