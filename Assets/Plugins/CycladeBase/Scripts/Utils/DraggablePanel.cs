using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CycladeBase.Utils
{
    public class DraggablePanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private float padding = 10;

        private RectTransform _popupTransform;

        private Vector2 _halfSize;
        private Vector2 _normalizedPosition;

        private RectTransform _canvasTR;
        private IEnumerator _moveToPosCoroutine;

        private void Awake()
        {
            _popupTransform = (RectTransform)transform;
            _canvasTR = transform.GetComponentInParent<Canvas>().gameObject.GetComponent<RectTransform>();

            Vector2 pos = _popupTransform.anchoredPosition;
            if (pos.x != 0f || pos.y != 0f)
                _normalizedPosition = pos.normalized;
            else
                _normalizedPosition = new Vector2(0.5f, 0f);
        }

        private IEnumerator MoveToPosAnimation(Vector2 targetPos)
        {
            float modifier = 0f;
            Vector2 initialPos = _popupTransform.anchoredPosition;

            while (modifier < 1f)
            {
                modifier += 4f * Time.unscaledDeltaTime;
                _popupTransform.anchoredPosition = Vector2.Lerp(initialPos, targetPos, modifier);

                yield return null;
            }
        }

        public void OnUpdatedSize()
        {
            CheckCoroutine();
            StartCoroutine(OnUpdatedSizeCoroutine());
        }

        private IEnumerator OnUpdatedSizeCoroutine()
        {
            yield return new WaitForEndOfFrame();

            UpdateHalfSize();
            UpdatePosition(true);
        }

        public void OnBeginDrag(PointerEventData data)
        {
            UpdateHalfSize();

            if (_moveToPosCoroutine != null)
            {
                StopCoroutine(_moveToPosCoroutine);
                _moveToPosCoroutine = null;
            }
        }

        private void UpdateHalfSize()
        {
            _halfSize = _popupTransform.sizeDelta * 0.5f + Vector2.one * padding;
        }


        public void OnDrag(PointerEventData data)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTR, data.position, data.pressEventCamera, out var localPoint))
                _popupTransform.anchoredPosition = localPoint;
        }


        public void OnEndDrag(PointerEventData data)
        {
            UpdatePosition(false);
        }


        private void UpdatePosition(bool immediately)
        {
            var canvasRawSize = _canvasTR.rect.size;

            float canvasWidth = canvasRawSize.x;
            float canvasHeight = canvasRawSize.y;

            float canvasBottomLeftX = 0f;
            float canvasBottomLeftY = 0f;

#if UNITY_2017_2_OR_NEWER && ( UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS )
            var safeArea = Screen.safeArea;

            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            canvasWidth *= safeArea.width / screenWidth;
            canvasHeight *= safeArea.height / screenHeight;

            canvasBottomLeftX = canvasRawSize.x * (safeArea.x / screenWidth);
            canvasBottomLeftY = canvasRawSize.y * (safeArea.y / screenHeight);
#endif

            var pos = canvasRawSize * 0.5f + (immediately ? new Vector2(_normalizedPosition.x * canvasWidth, _normalizedPosition.y * canvasHeight) : (_popupTransform.anchoredPosition - new Vector2(canvasBottomLeftX, canvasBottomLeftY)));

            float distToLeft = pos.x;
            float distToRight = canvasWidth - distToLeft;

            float distToBottom = pos.y;
            float distToTop = canvasHeight - distToBottom;

            float horDistance = Mathf.Min(distToLeft, distToRight);
            float vertDistance = Mathf.Min(distToBottom, distToTop);

            if (horDistance < vertDistance)
            {
                if (distToLeft < distToRight)
                    pos = new Vector2(_halfSize.x, pos.y);
                else
                    pos = new Vector2(canvasWidth - _halfSize.x, pos.y);

                pos.y = Mathf.Clamp(pos.y, _halfSize.y, canvasHeight - _halfSize.y);
            }
            else
            {
                if (distToBottom < distToTop)
                    pos = new Vector2(pos.x, _halfSize.y);
                else
                    pos = new Vector2(pos.x, canvasHeight - _halfSize.y);

                pos.x = Mathf.Clamp(pos.x, _halfSize.x, canvasWidth - _halfSize.x);
            }

            pos -= canvasRawSize * 0.5f;

            _normalizedPosition.Set(pos.x / canvasWidth, pos.y / canvasHeight);

            pos += new Vector2(canvasBottomLeftX, canvasBottomLeftY);


            CheckCoroutine();

            if (immediately)
            {
                _popupTransform.anchoredPosition = pos;
            }
            else
            {
                _moveToPosCoroutine = MoveToPosAnimation(pos);
                StartCoroutine(_moveToPosCoroutine);
            }
        }

        private void CheckCoroutine()
        {
            if (_moveToPosCoroutine != null)
            {
                StopCoroutine(_moveToPosCoroutine);
                _moveToPosCoroutine = null;
            }
        }
    }
}