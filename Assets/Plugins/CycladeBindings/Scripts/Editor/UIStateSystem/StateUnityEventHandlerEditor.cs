using CycladeBindings.UIStateSystem.Base;
using CycladeBindings.UIStateSystem.Elements;
using UnityEditor;

namespace CycladeBindingsEditor.UIStateSystem
{
    [CustomEditor(typeof(StateUnityEventHandler))]
    public class StateUnityEventHandlerEditor : BaseActivatableStateEditor
    {
        protected override string GroupSelectorTitle => "These events will be invoked with true when ";
        protected override string StateSelectorTitle => !((BaseActivatableState)target).isInverse ?  "is equal to" : "is not equal to";

        protected override string SelectorPostTitle => "Otherwise, will be invoked with false";
    }
}