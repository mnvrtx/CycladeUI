using CycladeBase.Models;
using CycladeBase.Utils;
using CycladeBase.Utils.Logging;
using CycladeUI.Popups.System;
using UnityEngine;
using UnityEngine.UI;

namespace CycladeUI
{
    public class UITestController : MonoBehaviour
    {
        private static readonly Log log = new(nameof(UITestController));
        
        [CycladeHelpBox("Setup settings and press play on your test scene.")]
        public string stub;

        [SerializeField] private bool disableMock;
        [SerializeField] private Image optionalMock;

        public DebugSafeAreaSettings debugSafeArea;
        public BasePopup testPopup;
        public PopupSystem popupSystem;

        public T StartTest<T>() where T : BasePopup
        {
            if (!popupSystem)
            {
                log.Error($"Not found popupSystem. Please set.", gameObject);
                return null;
            }
            
            if (!testPopup)
            {
                log.Error($"Not found testPopup. Please set.", gameObject);
                return null;
            }
            
            testPopup.SetActive(false);
            var popup = (T)popupSystem.ShopPopupInternal(testPopup.name, testPopup, debugSafeArea);
            if (optionalMock && !debugSafeArea.enabled && !disableMock)
            {
                optionalMock.transform.SetParent(popup.transform);
                optionalMock.rectTransform.ToInitial();
                optionalMock.rectTransform.SetParentImitate();
                optionalMock.transform.SetAsLastSibling();
                optionalMock.SetActive(true);
            }
            else
            {
                optionalMock.SetActive(false);
            }

            return popup;
        }
    }
}