using UnityEngine;

namespace CycladeBase.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GlobalCycladeSettings", menuName = "CycladeUI/GlobalCycladeSettings", order = 0)]
    public class GlobalCycladeSettings : ScriptableObject
    {
        public bool initializePerformanceAnalyzer = true;
    }
}