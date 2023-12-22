using UnityEngine;
using UnityEngine.UI;

namespace CycladeUI.Popups.PrefEditor.Utils
{
    public class SelectableImage : BaseSwitcher
    {
        public bool IsEnabled { get; private set; }

        [SerializeField] private Sprite[] images;
        [SerializeField] private Image image;

        public override void Select(bool isEnabled)
        {
            IsEnabled = isEnabled;
            image.sprite = images[IsEnabled ? 1 : 0];
        }
        
        public override void Select(int idx)
        {
            IsEnabled = idx != 0;
            image.sprite = images[idx];
        }

        [ContextMenu("Flip active")]
        public void Flip()
        {
            Select(!IsEnabled);
        }
    }
}