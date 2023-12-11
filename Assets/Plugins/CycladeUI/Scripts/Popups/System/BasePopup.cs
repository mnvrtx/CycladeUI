using System;
using CycladeUI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace CycladeUI.Popups.System
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class BasePopup : MonoBehaviour
    {
        public Button closeBtn;
        public bool needBackground = true;
        public bool needSafeArea = true;
        
        public SubscriptionHandlers OnClose = new();

        [NonSerialized] public IPopupAnimation Animation;
        [NonSerialized] public RectTransform Holder;
        [NonSerialized] public bool NonClosable;
        [NonSerialized] public bool CloseByClickOnBack = true;

        [NonSerialized] public PopupSystem PopupSystem;
    }
}