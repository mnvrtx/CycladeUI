using CycladeBase.PerformanceAnalyzer.Trackers;
using CycladeLocalization;
using CycladeStorage;
using Shared.Utils.Logging;
using UnityEngine;
using LogType = Shared.Utils.Logging.LogType;

namespace CycladeUIExample
{
    public static class StaticEntryPoint
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void OnBeforeSplashScreen()
        {
            SetupCycladeAnalyzer();
            LocalStorage.IsDebug = true;
            LogSystem.Initialize(LogType.Trace, new UnityLogger());
            Localization.I.Setup();
        }

        private static void SetupCycladeAnalyzer()
        {
            //You can set your default values here

            CycladeGeneralRecorders.DefaultNormalTrianglesCount = 7000;
            CycladeGeneralRecorders.DefaultCriticalTrianglesCount = 15000;

            CycladeGeneralRecorders.DefaultCriticalMemoryMB =
#if UNITY_EDITOR
            13_000;
#else
            1800;
#endif
            
            CycladeGeneralRecorders.DefaultNormalMemoryMB = 
#if UNITY_EDITOR
            10_000;
#else
            1300;
#endif 
        }
    }
}