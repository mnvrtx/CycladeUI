using UnityEngine;

namespace CycladeBindings.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GlobalCycladeBindingsSettings", menuName = "CycladeBindings/GlobalSettings", order = 0)]
    public class GlobalCycladeBindingsSettings : ScriptableObject
    {
        public const string DefaultPath = "Scripts/GeneratedBindings";
        public string baseBindingsPath = DefaultPath;
    }
}