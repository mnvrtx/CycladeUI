using System;
using CycladeBase.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace CycladeUI.Popups.System
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class BasePopup : MonoBehaviour
    {
        public Button optionalCloseBtn;
        public BasePopupAnimation optionalAnimation;
        public RectTransform optionalOutsideSafeArea;

        public CanvasGroup optionalCanvasGroup;
        [CycladeHelpBox("Using a canvas group is important for disabling UI buttons during animations")] public string stub;
        
        public bool isFullScreenPopup;
        [CycladeHelpBox("If \"isFullScreenPopup\" flag is set to true, then this popup will become inactive when another full-screen popup is opened on top of it.")] public string stub2;

        public bool needBackground = true;
        public Color backgroundColor = new(0f, 0f, 0f, 0.74f);
        public bool needSafeArea = true;
        
        public bool unloadAssetsAfterClose;
        [CycladeHelpBox("Unload unused assets after close \"OnDemand\" popup")] public string stub3;

        public readonly ClSubscriptionHandlers OnClose = new();
        public readonly ClSubscriptionHandlers OnCloseAfterAnimation = new(); 
        public readonly ClSubscriptionHandlers OnCloseAfterAnimationRobust = new();

        [NonSerialized] public RectTransform Holder;
        [NonSerialized] public bool NonClosable;
        [NonSerialized] public bool NonClosableByClickOnBack;

        [NonSerialized] public PopupSystem PopupSystem;
        [NonSerialized] public SetActiveDelayed SetActiveDelayed;
    }
}