using System;
using System.Linq;
using CycladeBase.Utils.Logging;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CycladeUI.Models
{
    [Serializable]
    public class PopupEntry
    {
        public PopupInfo type;
        public string assetPath;

        public PopupEntry(PopupInfo type)
        {
            this.type = type;
        }

        public void TryToFindAndSetAssetPathByType(Log log)
        {
            assetPath = TryToFindAndSetAssetPathByType(type.assemblyName, type.fullName, log);
        }

        public static string TryToFindAndSetAssetPathByType(string assemblyName, string typeFullName, Log log)
        {
#if UNITY_EDITOR
            var foundType = PopupInfo.TryFind(assemblyName, typeFullName);
            if (foundType == null)
                return string.Empty;

            var foundPrefabs = Resources.LoadAll("", foundType);
            if (foundPrefabs != null && foundPrefabs.Length > 0)
            {
                Object foundPrefab;
                if (foundPrefabs.Length > 1)
                {
                    foundPrefab = foundPrefabs.FirstOrDefault(q => q.name.StartsWith("[MAIN]"));
                    if (foundPrefab == null)
                    {
                        log.Warn($"Multiple prefabs of type {typeFullName} have been found, and none start with the [MAIN] prefix. The first one, {foundPrefabs[0].name}, will be selected.");
                        foundPrefab = foundPrefabs[0];
                    }
                }
                else
                {
                    foundPrefab = foundPrefabs[0];
                }
                
                var assetPath = UnityEditor.AssetDatabase.GetAssetPath(foundPrefab);

                return assetPath;
            }

            return string.Empty;
#else
            return string.Empty;
#endif
        }
    }
}