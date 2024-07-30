using CycladeBindings.UIStateSystem.Elements;
using CycladeBindingsEditor.UIStateSystem.Base;
using UnityEditor;

namespace CycladeBindingsEditor.UIStateSystem
{
    [CustomEditor(typeof(ActivatableButton))]
    public class ActivatableButtonEditor : BaseActivatableStateEditor
    {
        protected override string GroupSelectorTitle => "The interactable parameter of\nButton will be set to true when";
        protected override string StateSelectorTitle => !((ActivatableButton)target).isInverse ?  "is equal to" : "is not equal to";
    }
}