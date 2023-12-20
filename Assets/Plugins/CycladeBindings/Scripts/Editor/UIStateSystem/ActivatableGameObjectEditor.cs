using CycladeBindings.UIStateSystem.Base;
using CycladeBindings.UIStateSystem.Elements;
using UnityEditor;

namespace CycladeBindingsEditor.UIStateSystem
{
    [CustomEditor(typeof(ActivatableGameObject))]
    public class ActivatableGameObjectEditor : BaseActivatableStateEditor
    {
        protected override string GroupSelectorTitle => "This GameObject will be activated only when";
        protected override string StateSelectorTitle => !((BaseActivatableState)target).isInverse ?  "is equal to" : "is not equal to";
    }
}