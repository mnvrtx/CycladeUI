using CycladeBase.PerformanceAnalyzer.Trackers.Base;
using CycladeBase.Utils;
using UnityEngine;

namespace CycladeBase.PerformanceAnalyzer.View
{
    public class ValueGraphsView : MonoBehaviour
    {
        public ViewInstances<ValueGraph> valueGraphs;

        public void Initialize(params ITrackerResultsProvider[] providers)
        {
            valueGraphs.Clear();

            foreach (var provider in providers)
            {
                foreach (var kvp in provider.Results)
                {
                    var data = kvp.Value;

                    var graph = valueGraphs.GetNew(transform);
                    graph.SetActive(false);
                    graph.Initialize(data.Name, data.ValSuffix);

                    data.ValueGraph = new ValueGraphWrapper(graph, data);
                    graph.MiddleValueThreshold = data.MiddleValueThreshold;
                    graph.LowValueThreshold = data.LowValueThreshold;
                    graph.TopValueIsGood = data.TopValueIsGood;
                }
            }
        }
    }
}