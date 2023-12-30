using CycladeBindings.UIStateSystem.Base;
using CycladeBindings.UIStateSystem.Models;
using UnityEngine;

namespace CycladeBindings.UIStateSystem.Elements
{
    public class SelectableUnityEvent : BaseSelectableState<GameObject, UnityEventState>
    {
        protected override void OnSelected(GameObject element, UnityEventState state)
        {
            state.unityEvent.Invoke();
        }
    }
}