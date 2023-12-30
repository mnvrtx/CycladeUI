using CycladeBase.Utils;
using UnityEngine;

namespace CycladeBindings.UIStateSystem.Base
{
    public abstract class BaseStatefulElement : MonoBehaviour
    {
        public int order;
        [CycladeHelpBox("Optional State Execution Order")] public string stub2;
        
        public bool isInverse;
        public void Select(bool isEnabled) => Select(isEnabled ? "true" : "false");
        public abstract void Select(string stateName);
    }
}