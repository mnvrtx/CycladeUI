using CycladeLocalization;
using CycladeStorage;
using UnityEngine;

namespace CycladeUIExample
{
    public static class StaticEntryPoint
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void OnBeforeSplashScreen()
        {
            LocalStorage.IsDebug = true;
            Localization.I.Setup();
        }
    }
}