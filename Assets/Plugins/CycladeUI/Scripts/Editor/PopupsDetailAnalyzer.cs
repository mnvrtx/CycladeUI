using System.Linq;
using CycladeUI.Models;
using CycladeUI.Popups.System;
using CycladeUI.ScriptableObjects;
using CycladeUI.Utils.Logging;
using UnityEditor;
using UnityEngine;

namespace CycladeUIEditor
{
    public static class PopupsDetailAnalyzer
    {
        public static void AnalyzeAll(Log log)
        {
            var popupSystems = EditorCommon.FindScriptableObjects<PopupSystemSettings>();
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
                var asset = AssetDatabase.LoadAssetAtPath<BasePopup>(load.assetPath);
                var type = load.TryFindType();

                log.Trace($"{settings.name}. {i}. asset: {asset}. type: {type}");

                if (asset == null && type != null) //check asset renamed or moved
                {
                    var firstPath = load.assetPath;
                    load.assetPath = PopupEntry.TryToFindAndSetAssetPathByType(load.assemblyName, load.typeFullName, log);
                    asset = AssetDatabase.LoadAssetAtPath<BasePopup>(load.assetPath);
                    if (asset == null) //asset not found
                    {
                        log.Warn($"Asset by path {load.assetPath} (first: {firstPath}) in {settings.name} not found. Remove entry (auto)");
                        RemoveAndDecrI(ref settings.selectedPopups, ref i);
                    }
                    else
                    {
                        log.Debug($"Asset by path {load.assetPath} in {settings.name} updated");
                    }
                    Save(settings);
                }
                else if (type == null && asset != null) //check type renamed or moved
                {
                    var assetType = asset.GetType();
                    load.assemblyName = assetType.Assembly.FullName;
                    load.typeFullName = assetType.FullName;
                    Save(settings);
                    log.Debug($"Asset with type {assetType.FullName} in {settings.name} updated");
                }
                else if (type == null && asset == null) //if all is null
                {
                    log.Warn($"Not found type ({load.typeFullName}) and asset ({load.assetPath}) in {settings.name}. Remove entry (auto)");
                    RemoveAndDecrI(ref settings.selectedPopups, ref i);
                    Save(settings);
                }
            }
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