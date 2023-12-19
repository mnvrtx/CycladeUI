using System.Collections.Generic;
using CycladeBase.Utils.Logging;
using CycladeBaseEditor.Editor;
using CycladeUI.ScriptableObjects;
using UnityEditor;

namespace CycladeUIEditor
{
    public class CycladeAssetPostProcessor : AssetPostprocessor
    {
        private static readonly Log log = new(nameof(CycladeAssetPostProcessor));

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (CheckPopup(out var list))
                log.Debug($"{list.Count} scanned popups in settings");
            PopupsDetailAnalyzer.AnalyzeAll(log);
        }

        private static bool CheckPopup(out List<PopupEntryData> list)
        {
            list = new List<PopupEntryData>();

            var settings = CycladeEditorCommon.TryFindGlobalSettings<GlobalPopupSystemSettings>();
            if (settings == null)
                return false;

            PopupsScanner.Scan(settings, list, log);

            return true;
        }
    }
}