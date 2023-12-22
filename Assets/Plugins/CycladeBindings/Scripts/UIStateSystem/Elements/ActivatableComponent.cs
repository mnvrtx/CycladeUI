using CycladeBindings.UIStateSystem.Base;
using UnityEngine;

namespace CycladeBindings.UIStateSystem.Elements
{
    [AddComponentMenu("UIStateSystem/" + nameof(ActivatableComponent))]
    public class ActivatableComponent : BaseActivatableState
    {
        public Behaviour behaviour;

        public override void Select(string stateName) => behaviour.enabled = !isInverse ? stateName == state : stateName != state;
    }
}