using System.Linq;
using CycladeBase.Utils.Logging;
using CycladeBaseEditor.Editor;
using CycladeUI.Models;
using CycladeUI.Popups.System;
using CycladeUI.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace CycladeUIEditor
{
    public static class PopupsDetailAnalyzer
    {
        public static void AnalyzeAll(Log log)
        {
            var popupSystems = CycladeEditorCommon.FindScriptableObjects<PopupSystemSettings>();
            log.PrintData(string.Join(", ", popupSystems.Select(q => q.name)), "PopupSystems");

            foreach (var popupSystem in popupSystems) 
                AnalyzeOne(popupSystem, log);
        }

        public static void AnalyzeOne(PopupSystemSettings settings, Log log)
        {
            settings.FillFromSerialized();
            for (var i = 0; i < settings.selectedPopups.Length; i++)
            {
                var load = settings.selectedPopups[i];

                //validate asset and type
                var asset = TryLoadAsset(load);
                var type = load.TryFindType();

                log.Trace($"{settings.name}. {i}. asset: {asset}. type: {type}");

                if (asset == null && type != null) //check asset renamed or moved
                {
                    var firstPath = load.assetPath;
                    load.assetPath = PopupEntry.TryToFindAndSetAssetPathByType(load.assemblyName, load.typeFullName, log);
                    asset = TryLoadAsset(load);
                    if (asset == null) //asset not found
                    {
                        log.Warn($"Asset by path {load.assetPath} (first: {firstPath}) in {settings.name} not found. Remove entry (auto)");
                        RemoveAndDecrI(ref settings.selectedPopups, ref i);
                    }
                    else
                    {
                        log.Info($"Asset by path {load.assetPath} in {settings.name} updated");
                    }
                    Save(settings);
                }
                else if (type == null && asset != null) //check type renamed or moved
                {
                    var assetType = asset.GetType();
                    load.assemblyName = assetType.Assembly.FullName;
                    load.typeFullName = assetType.FullName;
                    Save(settings);
                    log.Info($"Asset with type {assetType.FullName} in {settings.name} updated");
                }
                else if (type == null && asset == null) //if all is null
                {
                    log.Warn($"Not found type ({load.typeFullName}) and asset ({load.assetPath}) in {settings.name}. Remove entry (auto)");
                    RemoveAndDecrI(ref settings.selectedPopups, ref i);
                    Save(settings);
                }
            }
        }

        private static BasePopup TryLoadAsset(PopupLoadEntry load) => AssetDatabase.LoadAssetAtPath<BasePopup>(load.assetPath);

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