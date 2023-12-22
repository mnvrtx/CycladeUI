using UnityEngine;

namespace CycladeBindings.UIStateSystem.Base
{
    public abstract class BaseStatefulElement : MonoBehaviour
    {
        public int order;
        public bool isInverse;
        public void Select(bool isEnabled) => Select(isEnabled ? "true" : "false");
        public abstract void Select(string stateName);
    }
}