using System;
using CycladeUI.Utils.Logging;
using UnityEngine;

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
                var assetPath = UnityEditor.AssetDatabase.GetAssetPath(foundPrefabs[0]);

                if (foundPrefabs.Length > 1)
                    log.Warn($"More than one prefab of type {typeFullName} found. Pick the first one: {foundPrefabs[0].name}");

                return assetPath;
            }

            return string.Empty;
#else
            return string.Empty;
#endif
        }
    }
}