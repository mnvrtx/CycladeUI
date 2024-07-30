using Shared.Utils.Logging;
using UnityEngine;

namespace CycladeUIExample.Popups.Shop
{
    public class ShopCardHandler : MonoBehaviour
    {
        private static readonly Log log = new(nameof(ShopCardHandler));
        
        public void U_SomeStateCallback()
        {
            log.Info("SomeStateCallback");
        }

        public void U_OtherStateCallback()
        {
            log.Info("OtherStateCallback");
        }
    }
}