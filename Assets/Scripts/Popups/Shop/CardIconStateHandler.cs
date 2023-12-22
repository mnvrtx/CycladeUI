using CycladeBase.Utils.Logging;
using UnityEngine;

namespace CycladeUIExample.Popups.Shop
{
    public class CardIconStateHandler : MonoBehaviour
    {
        private static readonly Log log = new(nameof(CardIconStateHandler));

        public void U_OnStateChanged(bool isActive)
        {
            log.Info($"{nameof(CardIconStateHandler)}.{nameof(U_OnStateChanged)}. isActive: {isActive}");
        }
    }
}