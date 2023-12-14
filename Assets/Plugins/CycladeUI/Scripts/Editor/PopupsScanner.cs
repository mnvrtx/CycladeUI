using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CycladeBase.Utils;
using CycladeBase.Utils.Logging;
using CycladeUI.Models;
using CycladeUI.Popups.System;
using CycladeUI.ScriptableObjects;
using UnityEngine;

namespace CycladeUIEditor
{
    public static class PopupsScanner
    {
        public static void Scan(GlobalPopupSystemSettings settings, List<PopupEntryData> foundEntryDataList, Log log)
        {
            RescanPopups(settings, foundEntryDataList);
            FindAndFillByTypes(foundEntryDataList, log);
        }

        private static void RescanPopups(GlobalPopupSystemSettings settings, List<PopupEntryData> foundEntryDataList)
        {
            FindAvailableAssemblies(settings);
            FindPopups(settings, foundEntryDataList);
        }

        private static void FindAvailableAssemblies(GlobalPopupSystemSettings settings)
        {
            settings.assemblies = CycladeHelpers.FindAssembliesWith(t => t.IsSubclassOf(typeof(BasePopup)))
                .Select(q => q.FullName)
                .ToList();
        }

        private static void FindPopups(GlobalPopupSystemSettings settings, List<PopupEntryData> foundEntryDataList)
        {
            settings.entries = new List<PopupEntry>();
            foundEntryDataList.Clear();

            foreach (string assemblyName in settings.assemblies)
            {
                try
                {
                    var types = Assembly.Load(assemblyName).GetTypes();

                    foreach (var type in types)
                    {
                        if (type.IsSubclassOf(typeof(BasePopup)))
                        {
                            var typeFullName = type.FullName;

                            var info = new PopupInfo(assemblyName, typeFullName);
                            var entry = new PopupEntry(info);
                            settings.entries.Add(entry);
                            foundEntryDataList.Add(new PopupEntryData(entry));
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            } 

            foreach (var entryData in foundEntryDataList) 
                entryData.Type = entryData.Entry.type.TryFind();
        }

        private static void FindAndFillByTypes(List<PopupEntryData> foundEntryDataList, Log log)
        {
            foreach (var data in foundEntryDataList)
            {
                if (!string.IsNullOrEmpty(data.Entry.assetPath))
                    continue;

                data.Entry.TryToFindAndSetAssetPathByType(log);
            }
        }
    }
}