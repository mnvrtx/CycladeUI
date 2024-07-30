using System.Linq;
using CycladeBindings.UIStateSystem.Base;
using UnityEngine;

namespace CycladeBindings.UIStateSystem.Elements
{
    [AddComponentMenu("UIStateSystem/" + nameof(ActivatableCanvasGroupInteractable))]
    public class ActivatableCanvasGroupInteractable : BaseActivatableState
    {
        public override void Select(string stateName) => GetMyComponent<CanvasGroup>().interactable = !isInverse ? states.Contains(stateName) : !states.Contains(stateName);
    }
}