using CycladeBase.PerformanceAnalyzer.Trackers.Base;
using CycladeBase.Utils;
using Solonity.View.Utils;
using UnityEngine;

namespace CycladeBase.PerformanceAnalyzer.View
{
    public class ValueGraphWrapper : IValueGraph
    {
        private readonly ValueGraph _valueGraph;
        private readonly AnalyzeData _data;
        private readonly DraggablePanel _draggablePanel;
        private readonly GameObject _gameObject;

        public ValueGraphWrapper(ValueGraph valueGraph, AnalyzeData data, DraggablePanel draggablePanel)
        {
            _valueGraph = valueGraph;
            _gameObject = _valueGraph.gameObject;
            _data = data;
            _draggablePanel = draggablePanel;
        }

        public void Update()
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (!_gameObject.activeSelf && (_data.ElapsedResult != float.MinValue || _data.ElapsedResultLong != long.MinValue))
            {
                _valueGraph.SetActive(true);
                _draggablePanel.OnUpdatedSize();
            }

            if (_data.ValSuffix.IsLong)
                _valueGraph.ProcessLong(_data.ElapsedResultLong);
            else
                _valueGraph.Process(_data.ElapsedResult);
        }
    }
}