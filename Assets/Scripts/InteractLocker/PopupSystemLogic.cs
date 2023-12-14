using CycladeUI.Popups.System;
using UnityEngine;

namespace CycladeUIExample.InteractLocker
{
    public class PopupSystemLogic : PopupSystemLogicBase
    {
        [SerializeField] private GameObject interactionLocker;

        public override bool IsLocked() => interactionLocker.activeSelf;

        public override void ShowExitDialogOnBack(PopupSystem popupSystem)
        {
            popupSystem.ShowConfirmation("Are you sure you want to go out?", "yes", "no", isConfirm =>
            {
                if (isConfirm)
                    Application.Quit();
            });
        }
    }
}