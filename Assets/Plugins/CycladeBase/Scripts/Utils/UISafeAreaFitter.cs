using CycladeBase.Utils.Logging;
using CycladeUI.Models;
using UnityEngine;

namespace CycladeBase.Utils
{
    public static class UISafeAreaFitter
    {
        private static readonly Log log = new(nameof(UISafeAreaFitter));

        public static void FitInSafeArea(this RectTransform trans, DebugSafeAreaSettings testSafeArea)
        {
            var rect = Screen.safeArea;

            trans.SetParentImitate();

            if (testSafeArea.enabled)
            {
                log.Debug($"Set test safe area with {testSafeArea.left} {testSafeArea.top} {testSafeArea.right} {testSafeArea.bot} offset");
                rect = new Rect(rect.x + testSafeArea.left,
                    rect.y + testSafeArea.bot,
                    rect.width - testSafeArea.right - testSafeArea.left,
                    rect.height - testSafeArea.top - testSafeArea.bot);
            }

            var anchorMin = rect.position;
            var anchorMax = rect.position + rect.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            
#if UNITY_IOS
            anchorMin.y /= 3;
#endif
            trans.anchorMin = anchorMin;
            trans.anchorMax = anchorMax;

            log.Debug($"Applied to {trans.name}: {rect} on full extents w={Screen.width}, h={Screen.height}");
        }
    }
}