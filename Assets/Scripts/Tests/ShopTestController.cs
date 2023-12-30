using CycladeUI;
using CycladeUIExample.Models;
using CycladeUIExample.Popups.Shop;
using UnityEngine;

namespace CycladeUIExample.Tests
{
    public class ShopTestController : MonoBehaviour
    {
        [SerializeField] private GeneralUITestController testController;

        private void Awake()
        {
            testController.ShowAndDebugPopup<ShopPopup>(popup =>
            {
                popup.Initialize(ProductsData.GetExampleMockDataFromServer());
            });
        }
    }
}