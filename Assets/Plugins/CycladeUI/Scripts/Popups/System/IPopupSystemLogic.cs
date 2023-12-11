namespace CycladeUI.Popups.System
{
    public interface IPopupSystemLogic
    {
        bool IsLocked();
        void ShowExitDialogOnBack(PopupSystem popupSystem);
    }
}