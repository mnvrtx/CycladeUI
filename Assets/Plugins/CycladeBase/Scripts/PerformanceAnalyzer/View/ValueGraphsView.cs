using System.Collections.Generic;
using CycladeBase.PerformanceAnalyzer.Trackers.Base;
using CycladeBase.Utils;
using UnityEngine;

namespace CycladeBase.PerformanceAnalyzer.View
{
    public class ValueGraphsView : MonoBehaviour
    {
        [SerializeField] private ViewInstances<ValueGraph> valueGraphs;
        [SerializeField] private DraggablePanel draggablePanel;

        private readonly List<MonoBehaviour> _behaviours = new();

        public void Initialize(params ITrackerResultsProvider[] providers)
        {
            valueGraphs.Initialize();

            foreach (var provider in providers)
            {
                foreach (var kvp in provider.Results)
                {
                    var data = kvp.Value;

                    var graph = valueGraphs.GetNew(transform);
                    graph.SetActive(false);

                    data.ValueGraph = new ValueGraphWrapper(graph, data, draggablePanel);
                    graph.MiddleValueThreshold = data.MiddleValueThreshold;
                    graph.LowValueThreshold = data.LowValueThreshold;
                    graph.TopValueIsGood = data.TopValueIsGood;

                    graph.Initialize(data, draggablePanel);
                }
                
                if (provider is MonoBehaviour behaviour)
                    _behaviours.Add(behaviour);
            }
            
            draggablePanel.OnUpdatedSize();
        }

        private void OnEnable()
        {
            foreach (var behaviour in _behaviours)
            {
                if (behaviour)
                    behaviour.SetActive(true);
            }
            draggablePanel.OnUpdatedSize();
        }
        
        private void OnDisable()
        {
            foreach (var behaviour in _behaviours)
            {
                if (behaviour)
                    behaviour.SetActive(false);
            }
        }
    }
}