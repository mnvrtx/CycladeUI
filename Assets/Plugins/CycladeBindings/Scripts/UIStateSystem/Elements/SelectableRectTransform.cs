using CycladeBase.Utils;
using CycladeBindings.UIStateSystem.Base;
using CycladeBindings.UIStateSystem.Models;
using UnityEngine;

namespace CycladeBindings.UIStateSystem.Elements
{
    [AddComponentMenu("UIStateSystem/" + nameof(SelectableRectTransform))]
    public class SelectableRectTransform : BaseSelectableState<RectTransform, RectTransformState>
    {
        protected override void OnSelected(RectTransform element, RectTransformState state)
        {
            CycladeHelpers.SetRectTransformValues(element, state.rectTransform);
        }

        [ContextMenu("Flip active")]
        public void Flip()
        {
            Select(!IsEnabled);
        }
    }
}