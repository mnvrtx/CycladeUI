using UnityEngine;

namespace CycladeUI.Popups.PrefEditor.Utils
{
    public abstract class BaseSwitcher : MonoBehaviour
    {
#pragma warning disable CS0108, CS0114
        public string name;
#pragma warning restore CS0108, CS0114
        public abstract void Select(bool isEnabled);
        public abstract void Select(int idx);
    }
}