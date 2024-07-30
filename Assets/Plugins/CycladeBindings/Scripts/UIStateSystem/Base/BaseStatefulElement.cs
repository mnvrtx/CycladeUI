using CycladeBase.Utils;
using UnityEngine;

namespace CycladeBindings.UIStateSystem.Base
{
    public abstract class BaseStatefulElement : MonoBehaviour
    {
        public int order;
        [CycladeHelpBox("Optional State Execution Order")] public string stub2;

        private Component _cachedComponent;
        
        public bool isInverse;
        public void Select(bool isEnabled) => Select(isEnabled ? "true" : "false");
        public abstract void Select(string stateName);

        protected T GetMyComponent<T>() where T : Component
        {
            if (_cachedComponent == null) 
                _cachedComponent = GetComponent<T>();
            return (T)_cachedComponent;
        }
    }
}