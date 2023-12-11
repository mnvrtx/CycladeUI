namespace CycladeUI.Popups.System
{
    public static class PopupSystemExt
    {
        public static BasePopup SetNonClosable(this BasePopup popup, bool nonClosable = true)
        {
            popup.NonClosable = nonClosable;
            return popup;
        }
        
        public static BasePopup SetNonCloseByBackClick(this BasePopup popup, bool closeByClickOnBack = false)
        {
            popup.CloseByClickOnBack = closeByClickOnBack;
            return popup;
        }
    }
}