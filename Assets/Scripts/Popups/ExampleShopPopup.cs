using CycladeUI.Popups.System;
using Cysharp.Threading.Tasks;
using GeneratedCycladeBindings;

namespace CycladeUIExample.Popups
{
    public class ExampleShopPopup : BasePopup
    {
        public ExampleShopBinding binding;

        private object _someExternalDependency;
        
        public void Initialize(object someExternalDependency)
        {
            _someExternalDependency = someExternalDependency;

            foreach (var txt in binding.ChestCountTxtList) 
                txt.text = "999";
            foreach (var txt in binding.ChestPriceTxtList) 
                txt.text = "888";
            foreach (var txt in binding.CreditsCountTxtList) 
                txt.text = "777";
            foreach (var txt in binding.CreditsPriceTxtList) 
                txt.text = "666";
            foreach (var txt in binding.DiamondPriceTxtList) 
                txt.text = "555";
            foreach (var txt in binding.DiamondCountTxtList) 
                txt.text = "444";
        }

        public void U_OpenAnotherShop()
        {
            PopupSystem.ShowConfirmation("are you sure?", "open", "no", confirm =>
            {
                if (confirm)
                {
                    PopupSystem.ShowFastFollowPopup<ExampleShopPopup>(p =>
                    {
                        p.Initialize(_someExternalDependency);
                    }).ToUniTask(this).Forget();
                }
            });
        }
        
        public void U_ShowDescription()
        {
            PopupSystem.ShowInfo("some description");
        }

        public void U_CloseAllShops()
        {
            PopupSystem.ShowInfo("Info popup, which does not close if you select \"only shop\" or close confirmation.");

            PopupSystem.ShowConfirmation("close all?", "yes", "only shop", confirm =>
            {
                if (confirm)
                    PopupSystem.CloseAllPopups();
                else
                    PopupSystem.ClosePopupsOfType<ExampleShopPopup>();
            });
        }
    }
}