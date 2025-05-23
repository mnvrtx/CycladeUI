using Shared.Utils.Logging;
using UnityEngine;

namespace CycladeBase.Utils
{
    public class TargetFps : MonoBehaviour
    {
        private static readonly Log log = new(nameof(TargetFps), CycladeDebugInfo.I);

        public int targetFramerate = 60;
        public int targetVSyncCount;

        private void Awake()
        {
            if (targetVSyncCount != -1)
                QualitySettings.vSyncCount = targetVSyncCount;
            U_Set(targetFramerate);
        }

        public void U_Set(int targetFps)
        {
            var t = targetFps != 0 ? targetFramerate : (int)Screen.currentResolution.refreshRateRatio.value;
            
            var before = Application.targetFrameRate;
            if (t == before)
                return;
            Application.targetFrameRate = t;

            log.Info($"set. {before} -> {Application.targetFrameRate}", gameObject);
        }

        [ContextMenu("Refresh")]
        public void Refresh()
        {
            Awake();
        }
    }
}