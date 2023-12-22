using CycladeBase.Utils;
using CycladeBindings.UIStateSystem.Base;
using UnityEngine;
using UnityEngine.Events;

namespace CycladeBindings.UIStateSystem.Elements
{
    [AddComponentMenu("UIStateSystem/" + nameof(StateUnityEventHandler))]
    public class StateUnityEventHandler : BaseActivatableState
    {
        [CycladeHelpBox("Please set the EditorAndRuntime option for events for testing purposes.")] public string stub;
        
        public UnityEvent<bool> unityEvent;

        public override void Select(string stateName) => unityEvent.Invoke(!isInverse ? stateName == state : stateName != state);
    }
}