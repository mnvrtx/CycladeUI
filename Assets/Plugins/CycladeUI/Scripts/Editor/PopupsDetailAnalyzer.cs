using System.Linq;
using CycladeBaseEditor;
using CycladeUI.Models;
using CycladeUI.ScriptableObjects;
using Shared.Utils.Logging;
using UnityEditor;

namespace CycladeUIEditor
{
    public static class PopupsDetailAnalyzer
    {
        public static void AnalyzeAll(GlobalPopupSystemSettings globalSettings, Log log)
        {
            var popupSystems = CycladeEditorCommon.FindScriptableObjects<PopupSystemSettings>();
            log.PrintData(string.Join(", ", popupSystems.Select(q => q.name)), "PopupSystems");

            foreach (var popupSystem in popupSystems) 
                AnalyzeOne(popupSystem, globalSettings, log);
        }

        public static void AnalyzeOne(PopupSystemSettings settings, GlobalPopupSystemSettings globalSettings, Log log)
        {
            settings.FillFromSerialized();
            bool needToSave = false;
            for (var i = 0; i < settings.selectedPopups.Length; i++)
            {
                var selectedEntry = settings.selectedPopups[i];

                //validate asset and type
                var entry = globalSettings.entries.FirstOrDefault(q => q.assetPath == selectedEntry.assetPath
                                                                       && q.type.assemblyName == selectedEntry.assemblyName
                                                                       && q.type.fullName == selectedEntry.typeFullName);
                if (entry == null)
                {
                    var entryByType = globalSettings.entries.FirstOrDefault(q => q.type.assemblyName == selectedEntry.assemblyName 
                                                                                 && q.type.fullName == selectedEntry.typeFullName);

                    if (entryByType != null)
                    {
                        log.Info($"Asset (type: {selectedEntry.typeFullName}) in {settings.name} updated. (assetPath: {selectedEntry.assetPath} -> {entryByType.assetPath})");
                        selectedEntry.assetPath = entryByType.assetPath;
                        needToSave = true;
                    }
                    else
                    {
                        var entryByAssetPath = globalSettings.entries.FirstOrDefault(q => q.assetPath == selectedEntry.assetPath);
                        if (entryByAssetPath != null)
                        {
                            log.Info($"Asset (assetPath: {selectedEntry.assetPath}) in {settings.name} updated. (new type: {entryByAssetPath.type.fullName})");
                            selectedEntry.typeFullName = entryByAssetPath.type.fullName;
                            selectedEntry.assemblyName = entryByAssetPath.type.assemblyName;
                            needToSave = true;
                        }
                        else
                        {
                            log.Warn($"Not found type ({selectedEntry.typeFullName}) and asset ({selectedEntry.assetPath}) in {settings.name}. Remove entry (auto)");
                            RemoveAndDecrI(ref settings.selectedPopups, ref i);
                            needToSave = true;
                        }
                    }
                }
            }
            
            if (needToSave)
                Save(settings);
        }

        private static void Save(PopupSystemSettings settings)
        {
            settings.selectedPopupsSerialized = PopupLoadEntry.ToSerialized(settings.selectedPopups);
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void RemoveAndDecrI<T>(ref T[] array, ref int i)
        {
            ArrayUtility.RemoveAt(ref array, i);
            i--;
        }
    }
}