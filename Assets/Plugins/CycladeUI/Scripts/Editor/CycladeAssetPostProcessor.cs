using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shared.Utils.Logging;
using CycladeBaseEditor;
using CycladeUI.Popups.System;
using CycladeUI.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace CycladeUIEditor
{
    public class CycladeAssetPostProcessor : AssetPostprocessor
    {
        private static Log _log;
        private static readonly StringBuilder _sb = new();

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (_log == null)
            {
                _log = new Log(nameof(CycladeAssetPostProcessor), CycladeDebugInfo.I);
            }
            
            bool needsUpdate = CheckAssets(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
            if (!needsUpdate)
                return;
            
            var globalSettings = CycladeEditorCommon.TryFindGlobalSettings<GlobalPopupSystemSettings>();

            var list = new List<PopupEntryData>();
            PopupsScanner.ScanAndSaveToSettings(globalSettings, list, _log);
            PopupsDetailAnalyzer.AnalyzeAll(globalSettings, _log);
        }

        private static bool CheckAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var result = false;
            foreach (string assetPath in importedAssets)
            {
                // Check if the asset is a prefab
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (prefab != null)
                {
                    // Check if the prefab has a component derived from BasePopup
                    if (HasDerivedComponent<BasePopup>(prefab))
                    {
                        result = true;
                        break;
                    }
                }

                // Check if the asset is a script
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                if (script != null)
                {
                    // Check if the script derives from BasePopup
                    if (typeof(BasePopup).IsAssignableFrom(script.GetClass()))
                    {
                        result = true;
                        break;
                    }
                }
            }

            _sb.Clear();
            _sb.AppendLine($"OnPostprocessAllAssets. needs update? {result}");

            if (importedAssets.Any())
                _sb.AppendLine("Imported Assets:\n" + string.Join("\n", importedAssets));
            if (deletedAssets.Any())
                _sb.AppendLine("Deleted Assets:\n" + string.Join("\n", deletedAssets));
            if (movedAssets.Any())
                _sb.AppendLine("Moved Assets:\n" + string.Join("\n", movedAssets.Select((t, i) => $"From: {movedFromAssetPaths[i]} To: {t}")));

            _log.Debug(_sb.ToString());

            return result;
        }

        static bool HasDerivedComponent<T>(GameObject prefab) where T : Component
        {
            T[] components = prefab.GetComponentsInChildren<T>(true);
            return components.Length > 0;
        }
    }
}