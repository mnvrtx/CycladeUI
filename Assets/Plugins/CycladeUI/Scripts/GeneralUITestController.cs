using System;
using CycladeBase.Models;
using CycladeBase.Utils;
using CycladeBase.Utils.Logging;
using CycladeUI.Popups.System;
using UnityEngine;
using UnityEngine.UI;

namespace CycladeUI
{
    public class GeneralUITestController : MonoBehaviour //todo: make this inheritable
    {
        private static readonly Log log = new(nameof(GeneralUITestController));

        public BasePopup testPopupTemplate;
        [CycladeHelpBox("Set up the testPopupTemplate and press play on your test scene. You can change \"debugSafeArea\" and \"disableMock\" in play mode.")] public string stub;

        [SerializeField] private bool disableMock;
        [SerializeField] private Image optionalMock;
        [SerializeField] private Sprite mockSprite;

        public DebugSafeAreaSettings debugSafeArea;

        public PopupSystem popupSystem;

        private BasePopup _popupInstance;

        private bool IsNeedMock => optionalMock && !debugSafeArea.enabled && !disableMock;

        public T ShowPopup<T>() where T : BasePopup
        {
            if (!popupSystem)
                throw new Exception($"Not found popupSystem. Please set.");

            if (!testPopupTemplate)
                throw new Exception($"Not found testPopup. Please set.");

            testPopupTemplate.SetActive(false);
            _popupInstance = popupSystem.ShopPopupInternal(testPopupTemplate.name, testPopupTemplate, debugSafeArea); 

            if (optionalMock)
            {
                optionalMock.transform.SetParent(_popupInstance.transform);
                optionalMock.rectTransform.ToInitial();
                optionalMock.rectTransform.StretchAcrossParent();
                optionalMock.transform.SetAsLastSibling();
                optionalMock.sprite = mockSprite;
                optionalMock.SetActive(false);    
            }

            return (T)_popupInstance;
        }

        private void Update()
        {
            var isNeedMock = IsNeedMock;
            if (optionalMock && optionalMock.gameObject.activeSelf != isNeedMock)
                optionalMock.SetActive(isNeedMock);

            if (_popupInstance)
                _popupInstance.GetComponent<RectTransform>().FitInSafeArea(debugSafeArea);
        }
    }
}