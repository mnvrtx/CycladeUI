namespace CycladeUI.Popups.System
{
    public static class PopupSystemExt
    {
        public static T SetNonClosable<T>(this T popup, bool nonClosable = true) where T : BasePopup
        {
            popup.NonClosable = nonClosable;
            return popup;
        }
        
        public static T SetNonClosableByClickOnBack<T>(this T popup, bool nonClosable = true) where T : BasePopup
        {
            popup.NonClosableByClickOnBack = nonClosable;
            return popup;
        }
    }
}