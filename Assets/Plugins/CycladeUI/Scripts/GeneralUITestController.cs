using System;
using CycladeBase.Models;
using CycladeBase.Utils;
using Shared.Utils.Logging;
using CycladeUI.Popups.System;
using Solonity.View.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace CycladeUI
{
    public class GeneralUITestController : MonoBehaviour
    {
        public BasePopup testPopupTemplate;
        [CycladeHelpBox("Set up the testPopupTemplate and press play on your test scene. You can change \"debugSafeArea\" and \"disableMock\" in play mode.")] public string stub;

        [SerializeField] private bool disableMock;
        [SerializeField] private Image optionalMock;
        [SerializeField] private Sprite mockSprite;

        public DebugSafeAreaSettings debugSafeArea;

        public PopupSystem popupSystem;

        private BasePopup _popupInstance;

        private bool IsNeedMock => optionalMock && !debugSafeArea.enabled && !disableMock;
        
        [CycladeHelpBox("If you enable safeArea, the mock, if present, will turn off.")] public string stub2;
        private DebugSafeAreaSettings _lastDebugSafeArea;

        public void ShowAndDebugPopup<T>(Action<T> onCreate) where T : BasePopup
        {
            if (!popupSystem)
                throw new Exception($"Not found popupSystem. Please set.");

            if (!testPopupTemplate)
                throw new Exception($"Not found testPopup. Please set.");

            testPopupTemplate.SetActive(false);
            _popupInstance = popupSystem.ShowAndDebugPopup((T)testPopupTemplate, debugSafeArea, onCreate); 

            if (optionalMock)
            {
                optionalMock.transform.SetParent(_popupInstance.transform);
                optionalMock.rectTransform.SetToInitial();
                optionalMock.rectTransform.StretchAcrossParent();
                optionalMock.transform.SetAsLastSibling();
                optionalMock.sprite = mockSprite;
                optionalMock.SetActive(false);    
            }
        }

        private void Update()
        {
            var isNeedMock = IsNeedMock;
            if (optionalMock && optionalMock.gameObject.activeSelf != isNeedMock)
                optionalMock.SetActive(isNeedMock);

            if (_popupInstance)
            {
                if (!debugSafeArea.Equals(_lastDebugSafeArea))
                {
                    _popupInstance.GetComponent<RectTransform>().FitInSafeArea(debugSafeArea);
                    _lastDebugSafeArea = new DebugSafeAreaSettings(debugSafeArea);
                }
            }
        }
    }
}