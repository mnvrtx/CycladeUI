using CycladeBindings.UIStateSystem.Base;
using CycladeBindings.UIStateSystem.Models;
using TMPro;
using UnityEngine;

namespace CycladeBindings.UIStateSystem.Elements
{
    [AddComponentMenu("UIStateSystem/" + nameof(SelectableTextColor))]
    public class SelectableTextColor : BaseSelectableState<TMP_Text, TextColorState>
    {
        protected override void OnSelected(TMP_Text element, TextColorState state)
        {
            element.color = state.color;            
        }

        [ContextMenu("Flip active")]
        public void Flip()
        {
            Select(!IsEnabled);
        }
    }
}