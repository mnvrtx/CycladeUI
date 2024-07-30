using Shared.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CycladeBase.Utils
{
    [RequireComponent(typeof(Selectable))]
    public class ButtonAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public float time = 0.3f;

        private bool _down;
        private float _cur;

        private Vector3 _min;
        private Vector3 _max;

        private void Awake()
        {
            _min = transform.localScale;
            _max = _min * 0.95f;
        }

        private void Update()
        {
            _cur += _down ? Time.deltaTime : -Time.deltaTime;
            _cur = Mathf.Clamp(_cur, 0f, time);

            var value = _down
                            ? EasingFunctions.EaseOutQuint(0f, 1f, _cur / time)
                            : EasingFunctions.EaseInQuint(0f, 1f, _cur / time);

            transform.localScale = Vector3.Lerp(_min, _max, value);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _down = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _down = false;
        }
    }
}