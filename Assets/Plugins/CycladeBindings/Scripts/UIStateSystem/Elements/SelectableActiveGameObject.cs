using CycladeBindings.UIStateSystem.Base;
using CycladeBindings.UIStateSystem.Models;
using UnityEngine;

namespace CycladeBindings.UIStateSystem.Elements
{
    public class SelectableActiveGameObject : BaseSelectableState<GameObject, IsActiveState>
    {
        protected override void OnSelected(GameObject element, IsActiveState state)
        {
            element.SetActive(state.isActive);
        }
    }
}