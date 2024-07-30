using System.Collections.Generic;
using System.Diagnostics;
using BaseShared;
using UnityEngine;

namespace Shared.Utils.Logging
{
    public class FieldsWatcher
    {
        public const int VariableLifetime = 3_000;

        public static bool IsClient;
        public static Dictionary<string, (string, long)> WatchingVariables = new();
        public static bool IsChanged;
        
        private static List<string> _keysToRemove = new();


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void OnBeforeSplashScreen()
        {
            IsClient = false;
            WatchingVariables = new Dictionary<string, (string, long)>();
            IsChanged = false;
            _keysToRemove = new List<string>();
            
        }
        // [ExecuteOnReload]
        // public static void ResetCache()
        // {
        //     
        // }
        
        [Conditional("UNITY_EDITOR")]
        public static void ClientWatch(string inName, object inValue)
        {
            if (!IsClient)
                return;

            WatchingVariables[inName] = (inValue.ToString(), GeneralTime.NowMs);
            IsChanged = true;
        }
        
        public static void RemoveOldEntries()
        {
            if (!IsClient)
                return;
            
            if (WatchingVariables.Count == 0)
                return;

            long currentUnixTimeMilliseconds = GeneralTime.NowMs;
            _keysToRemove.Clear();

            foreach (var entry in WatchingVariables)
            {
                long elapsedMilliseconds = currentUnixTimeMilliseconds - entry.Value.Item2;
                if (elapsedMilliseconds > VariableLifetime)
                    _keysToRemove.Add(entry.Key);
            }

            if (_keysToRemove.Count > 0)
                IsChanged = true;

            foreach (var key in _keysToRemove) 
                WatchingVariables.Remove(key);
        }
    }
}