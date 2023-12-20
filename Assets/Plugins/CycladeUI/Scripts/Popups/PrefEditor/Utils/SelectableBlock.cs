using UnityEngine;

namespace UtilsModule.View.UI.Switchers
{
    public class SelectableBlock : BaseSwitcher
    {
        public bool IsEnabled => blocks[1].activeSelf;

        [SerializeField] private GameObject[] blocks;
        
        private int _selected;

        public override void Select(bool isEnabled)
        {
            Select(isEnabled ? 1 : 0);
        }

        public override void Select(int idx)
        {
            _selected = idx;
            for (int i = 0; i < blocks.Length; i++)
                blocks[i].SetActive(i == _selected);
        }

        public T SelectAndGet<T>(int idx) where T : MonoBehaviour
        {
            _selected = idx;

            T t = null;
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].SetActive(i == _selected);
                if (i == _selected)
                    t = blocks[i].GetComponent<T>();
            }

            return t;
        }

        public T GetSelected<T>() where T : MonoBehaviour
        {
            return blocks[_selected].GetComponent<T>();
        }

        
        [ContextMenu("Flip active")]
        public void Flip()
        {
            Select(!IsEnabled);
        }
    }
}