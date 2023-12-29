using CycladeBase.PerformanceAnalyzer.Trackers;
using CycladeBase.PerformanceAnalyzer.View;
using UnityEngine;

namespace CycladeUIExample.Performance
{
    public class CycladeAnalyzerInitializer : MonoBehaviour
    {
        [SerializeField] private ValueGraphsView graphsView;
        
        private void Start()
        {
            // Initialize your trackers here
            graphsView.Initialize(CycladeGeneralRecorders.I, ExampleRecordersTracker.I, ExampleMsTracker.I, ExampleValueTracker.I);
        }
    }
}