using UnityEngine;

namespace CycladeUI.Popups.System
{
    public abstract class BasePopupAnimation : MonoBehaviour
    {
        public abstract float PlayForward();
        public abstract float PlayBackward();
    }
}