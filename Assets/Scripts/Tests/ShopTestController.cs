using CycladeUIExample.Models;
using CycladeUIExample.Popups.Shop;
using UnityEngine;

namespace CycladeUIExample.Tests
{
    public class ShopTestController : MonoBehaviour
    {
        [SerializeField] private ShopPopup popup;


        private void Awake()
        {
            popup.Initialize(ProductsData.GetMock());
        }
    }
}