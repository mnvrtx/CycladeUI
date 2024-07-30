using CycladeBindings.UIStateSystem.Elements;
using CycladeBindingsEditor.UIStateSystem.Base;
using UnityEditor;

namespace CycladeBindingsEditor.UIStateSystem
{
    [CustomEditor(typeof(ActivatableCanvasGroupInteractable))]
    public class ActivatableCanvasGroupInteractableEditor : BaseActivatableStateEditor
    {
        protected override string GroupSelectorTitle => "The interactable parameter of\nCanvasGroup will be set to true when";
        protected override string StateSelectorTitle => !((ActivatableCanvasGroupInteractable)target).isInverse ?  "is equal to" : "is not equal to";
    }
}