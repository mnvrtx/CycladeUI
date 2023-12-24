using UnityEngine;

namespace CycladeBase
{
    public static class StaticEntryPoint
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void OnBeforeSplashScreen()
        {
            
        }
    }
}