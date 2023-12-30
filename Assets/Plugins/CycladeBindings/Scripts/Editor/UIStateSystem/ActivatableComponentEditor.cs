using CycladeBindings.UIStateSystem.Base;
using CycladeBindings.UIStateSystem.Elements;
using CycladeBindingsEditor.UIStateSystem.Base;
using UnityEditor;

namespace CycladeBindingsEditor.UIStateSystem
{
    [CustomEditor(typeof(ActivatableComponent))]
    public class ActivatableComponentEditor : BaseActivatableStateEditor
    {
        protected override string GroupSelectorTitle => "This Component will be activated only when";
        protected override string StateSelectorTitle => !((BaseActivatableState)target).isInverse ?  "is equal to" : "is not equal to";
    }
}