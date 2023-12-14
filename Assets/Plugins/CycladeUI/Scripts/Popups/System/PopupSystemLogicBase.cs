using UnityEngine;

namespace CycladeUI.Popups.System
{
    public abstract class PopupSystemLogicBase : MonoBehaviour
    {
        public abstract bool IsLocked();
        public abstract void ShowExitDialogOnBack(PopupSystem popupSystem);
    }
}