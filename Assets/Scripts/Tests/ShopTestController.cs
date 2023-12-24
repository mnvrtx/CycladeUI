using CycladeUI;
using CycladeUIExample.Models;
using CycladeUIExample.Performance;
using CycladeUIExample.Popups.Shop;
using UnityEngine;
using static CycladeUIExample.Performance.ExampleMsTrackerValues;

namespace CycladeUIExample.Tests
{
    public class ShopTestController : MonoBehaviour
    {
        [SerializeField] private GeneralUITestController testController;

        private void Awake()
        {
            testController.ShowPopup<ShopPopup>().Initialize(ProductsData.GetMock());
        }

        private void Update()
        {
            ExampleMsTracker.I.BeginTrack(Logic);
            //some operations here
            ExampleMsTracker.I.FinishTrack(Logic);
        }
    }
}