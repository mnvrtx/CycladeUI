using CycladeUI.Popups.System;

namespace CycladeUI.Test.Popups
{
    public class ExampleShopPopup : BasePopup
    {
        public void Initialize(object someExternalDependency)
        {
            
        }
        
        public void U_ShowDescription()
        {
            PopupSystem.ShowInfo("coin description");
        }
    }
}