using CycladeBindings.UIStateSystem.Base;
using CycladeBindings.UIStateSystem.Models;
using UnityEngine;
using UnityEngine.UI;

namespace CycladeBindings.UIStateSystem.Elements
{
    [AddComponentMenu("UIStateSystem/" + nameof(SelectableImage))]
    public class SelectableImage : BaseSelectableState<Image, ImageState>
    {
        protected override void OnSelected(Image element, ImageState state)
        {
            element.sprite = state.image;
            element.color = state.color;
        }

        [ContextMenu("Flip active")]
        public void Flip()
        {
            Select(!IsEnabled);
        }
    }
}