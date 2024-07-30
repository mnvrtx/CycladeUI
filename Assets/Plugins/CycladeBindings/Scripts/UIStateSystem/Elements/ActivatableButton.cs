using System.Linq;
using CycladeBindings.UIStateSystem.Base;
using UnityEngine;
using UnityEngine.UI;

namespace CycladeBindings.UIStateSystem.Elements
{
    [AddComponentMenu("UIStateSystem/" + nameof(ActivatableButton))]
    public class ActivatableButton : BaseActivatableState
    {
        public override void Select(string stateName) => GetMyComponent<Button>().interactable = !isInverse ? states.Contains(stateName) : !states.Contains(stateName);
    }
}