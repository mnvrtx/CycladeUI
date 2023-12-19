// using System.Collections.Generic;
// using System.Linq;
// using CycladeBase.Utils.Logging;
// using CycladeBaseEditor.Editor;
// using CycladeUI.ScriptableObjects;
// using UnityEditor;

namespace CycladeUIEditor
{
    // [InitializeOnLoad]
    public static class EditorOnRecompile
    {
        // private static readonly Log log = new(nameof(EditorOnRecompile), true);
        //
        // static EditorOnRecompile() => 
        //     EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        //
        // private static void OnPlayModeStateChanged(PlayModeStateChange state)
        // {
        //     log.CleanQueue();
        //     switch (state)
        //     {
        //         case PlayModeStateChange.ExitingEditMode:
        //             if (CheckPopup(out var list)) 
        //                 log.Debug($"CheckPopups before ExitingEditMode. {list.Count} scanned popups in settings");
        //             PopupsDetailAnalyzer.AnalyzeAll(log);
        //             log.SaveQueue();
        //             break;
        //         case PlayModeStateChange.EnteredPlayMode:
        //             log.LoadQueue();
        //             log.DequeueAllLogs("ExitingEditMode");
        //             break;
        //     }
        // }
        //
        // private static bool CheckPopup(out List<PopupEntryData> list)
        // {
        //     list = new List<PopupEntryData>();
        //
        //     var settings = ClUIEditorCommon.TryFindGlobalSettings<GlobalPopupSystemSettings>();
        //     if (settings == null)
        //         return false;
        //
        //     PopupsScanner.Scan(settings, list, log);
        //
        //     return true;
        // }
    }
}