using UnityEditor;

namespace Shared.Utils.Logging
{
    public class CycladeDebugInfo : IDebugControlProvider
    {
        public const string DebugKey = "CycladeUIDebug";

        private static CycladeDebugInfo _instance;

        public static CycladeDebugInfo I => _instance ??= new CycladeDebugInfo();

        private static readonly Cache<bool> isDebugCache = new(() =>
        {
#if UNITY_EDITOR
            return SessionState.GetBool(DebugKey, false);
            // return true;
#else
            return false;
#endif
        });

        public static void ResetCache()
        {
            isDebugCache.ResetCache();
        }

        public bool IsDebug => isDebugCache;
    }
}