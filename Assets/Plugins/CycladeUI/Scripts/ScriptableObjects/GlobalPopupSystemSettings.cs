using System.Collections.Generic;
using CycladeUI.Models;
using UnityEngine;

namespace CycladeUI.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GlobalPopupSystemSettings", menuName = "CycladeUI/GlobalPopupSystemSettings", order = 0)]
    public class GlobalPopupSystemSettings : ScriptableObject
    {
        public List<string> assemblies;
        public List<PopupEntry> entries;
        public DebugSafeAreaSettings debugSafeAreaSettings;
    }
}