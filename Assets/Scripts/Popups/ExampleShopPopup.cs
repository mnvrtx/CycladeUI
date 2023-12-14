using CycladeUI.Popups.System;
using GeneratedCycladeBindings;

namespace CycladeUI.Test.Popups
{
    public class ExampleShopPopup : BasePopup
    {
        public ExampleShopBinding binding;
        
        public void Initialize(object someExternalDependency)
        {
            foreach (var image in binding.DiamondIconImgList)
            {
                image.sprite = null;
            }
        }
        
        public void U_ShowDescription()
        {
            PopupSystem.ShowInfo("coin description");
        }
    }
}