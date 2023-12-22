using CycladeUI;
using CycladeUIExample.Models;
using CycladeUIExample.Popups.Shop;
using UnityEngine;

namespace CycladeUIExample.Tests
{
    public class ShopTestController : MonoBehaviour
    {
        [SerializeField] private UITestController testController;

        private void Awake()
        {
            testController.StartTest<ShopPopup>().Initialize(ProductsData.GetMock());
        }
    }
}