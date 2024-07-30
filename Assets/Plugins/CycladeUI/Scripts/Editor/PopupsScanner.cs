using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Shared.Utils.Logging;
using CycladeUI.Models;
using CycladeUI.Popups.System;
using CycladeUI.ScriptableObjects;
using Shared.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CycladeUIEditor
{
    public static class PopupsScanner
    {
        public static void ScanAndSaveToSettings(GlobalPopupSystemSettings settings, List<PopupEntryData> foundEntryDataList, Log log)
        {
            var sw = Stopwatch.StartNew();

            var availableAssemblies = FindAvailableAssemblies();
            
            foundEntryDataList.Clear();
            settings.entries = FindPopups(availableAssemblies, foundEntryDataList, log);
            
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            log.Debug($"PopupsScanner. Scan and Save GlobalPopupSystemSettings. Elapsed: {sw.Elapsed.TotalMilliseconds:f4}ms");
        }

        private static List<string> FindAvailableAssemblies()
        {
            return CodeHelpers.FindAssembliesWith(t => t.IsSubclassOf(typeof(BasePopup)))
                .Select(q => q.FullName)
                .ToList();
        }

        private static List<PopupEntry> FindPopups(List<string> availableAssemblies, List<PopupEntryData> foundEntryDataList, Log log)
        {
            var entries = new List<PopupEntry>();

            foreach (string assemblyName in availableAssemblies)
            {
                var types = Assembly.Load(assemblyName).GetTypes();

                foreach (var type in types)
                {
                    if (!type.IsSubclassOf(typeof(BasePopup)))
                        continue;

                    var typeFullName = type.FullName;
                    var info = new PopupInfo(assemblyName, typeFullName);
                    var entry = new PopupEntry(info, TryToFindAssetPathByType(assemblyName, typeFullName, log));
                    entries.Add(entry);
                    var entryData = new PopupEntryData(entry);
                    entryData.Type = type;
                    foundEntryDataList.Add(entryData);
                }
            }

            return entries;
        }

        public static string TryToFindAssetPathByType(string assemblyName, string typeFullName, Log log)
        {
#if UNITY_EDITOR
            var foundType = PopupInfo.TryFind(assemblyName, typeFullName);
            if (foundType == null)
                return string.Empty;

            var foundPrefabs = Resources.LoadAll("", foundType).Where(prefab => prefab.GetType() == foundType).ToArray();
            if (foundPrefabs != null && foundPrefabs.Length > 0)
            {
                Object foundPrefab;
                if (foundPrefabs.Length > 1)
                {
                    foundPrefab = foundPrefabs.FirstOrDefault(q => q.name.StartsWith("[MAIN]"));
                    if (foundPrefab == null)
                    {
                        log.Warn($"Multiple prefabs of type {typeFullName} have been found, and none start with the [MAIN] prefix. The first one, {foundPrefabs[0].name}, will be selected.", foundPrefabs[0]);
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