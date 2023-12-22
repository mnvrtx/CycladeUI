using CycladeBindings.UIStateSystem.Base;
using UnityEngine;

namespace CycladeBindings.UIStateSystem.Elements
{
    [AddComponentMenu("UIStateSystem/" + nameof(ActivatableGameObject))]
    public class ActivatableGameObject : BaseActivatableState
    {
        public override void Select(string stateName) => gameObject.SetActive(!isInverse ? stateName == state : stateName != state);

        [ContextMenu("Flip active")]
        public void Flip() => Select(!gameObject.activeSelf);
    }
}