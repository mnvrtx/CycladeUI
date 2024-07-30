using UnityEngine;

namespace CycladeBase.Utils
{
    public static class RectTransformOverlapUtils
    {
        public static bool CheckOverlap(Camera camera, Transform transform, RectTransform rectTransform)
        {
            var rect = TransformToScreenSpace(rectTransform);
            var transformPoint = RectTransformUtility.WorldToScreenPoint(camera, transform.position);

            return rect.Contains(transformPoint);
        }

        public static bool CheckOverlap(RectTransform rt1, RectTransform rt2)
        {
            Rect rect1 = TransformToScreenSpace(rt1);
            Rect rect2 = TransformToScreenSpace(rt2);

            return rect1.Overlaps(rect2);
        }

        public static Rect TransformToScreenSpace(RectTransform rectTransform)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            Vector2 min = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
            Vector2 max = RectTransformUtility.WorldToScreenPoint(null, corners[2]);

            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }
    }
}