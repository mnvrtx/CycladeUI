using System;
using CycladeUI.Popups.System;
using CycladeUIExample.Models;
using CycladeUIExample.Performance;
using Cysharp.Threading.Tasks;
using GeneratedCycladeBindings;
using static GeneratedCycladeBindings.CardBinding;
using static CycladeUIExample.Performance.ExampleMsTrackerValues;

namespace CycladeUIExample.Popups.Shop
{
    public class ShopPopup : BasePopup
    {
        public ShopPopupBinding binding;

        private ProductsData _productsData;
        
        public void Initialize(ProductsData productsData)
        {
            _productsData = productsData;

            InitializeInGameProducts();
            InitializeIapProducts();
        }

        private void InitializeInGameProducts()
        {
            binding.ChestCardBindInstances.Clear();
            binding.CreditsCardBindInstances.Clear();

            foreach (var product in _productsData.InGameProducts)
            {
                var bindInstances = product.IsChest ? binding.ChestCardBindInstances : binding.CreditsCardBindInstances;
                var cardBinding = bindInstances.GetNew();
                GetAndFillProductInfo(product, cardBinding, out var cardType);
                var productType = Enum.Parse<ProductType>(product.InGameType.ToString());
                cardBinding.SetState(CurrencyType.Diamonds, productType, cardType);
                cardBinding.PriceTxt.text = product.Price.ToString();
            }
        }

        private void InitializeIapProducts()
        {
            binding.DiamondCardBindInstances.Clear();
            foreach (var product in _productsData.IapProducts)
            {
                var cardBinding = binding.DiamondCardBindInstances.GetNew();
                GetAndFillProductInfo(product, cardBinding, out var cardType);
                var productType = Enum.Parse<ProductType>(product.IapType.ToString());
                cardBinding.SetState(CurrencyType.Real, productType, cardType);
                cardBinding.PriceTxt.text = $"$ {product.Price.ToString()}";
            }
        }

        private static void GetAndFillProductInfo(BaseProduct product, CardBinding cardBinding, out CardType cardType)
        {
            cardType = !string.IsNullOrEmpty(product.AdditionalInfo) ? CardType.CardWithAdditionalInfo : CardType.Simple;
            cardBinding.AdditionalTextTxt.text = product.AdditionalInfo;
            cardBinding.CountTxt.text = product.Count.ToString();
        }
        
        
        private void Update()
        {
            ExampleMsTracker.I.BeginTrack(Logic);
            // Thread.Sleep(20); //FOR tests
            ExampleMsTracker.I.FinishTrack(Logic);
        }

        public void U_OpenAnotherShop()
        {
            PopupSystem.ShowConfirmation("are you sure?", "open", "no", confirm =>
            {
                if (confirm)
                {
                    PopupSystem.ShowFastFollowPopup<ShopPopup>(p =>
                    {
                        p.Initialize(_productsData);
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
                    PopupSystem.ClosePopupsOfType<ShopPopup>();
            });
        }
    }
}