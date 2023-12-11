using System.Collections.Generic;
using System.Linq;
using CycladeUI.ScriptableObjects;
using CycladeUI.Utils.Logging;
using UnityEditor;
using UnityEditor.Callbacks;

namespace CycladeUIEditor
{
    [InitializeOnLoad]
    public static class EditorOnRecompile
    {
        private static readonly Log log = new(nameof(EditorOnRecompile), true);

        static EditorOnRecompile() => 
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            log.CleanQueue();
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    if (CheckPopup(out var list)) 
                        log.Debug($"CheckPopups before ExitingEditMode. {list.Count} scanned popups in settings");
                    PopupsDetailAnalyzer.AnalyzeAll(log);
                    log.SaveQueue();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    log.LoadQueue();
                    log.DequeueAllLogs("ExitingEditMode");
                    break;
            }
        }
        
        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            var log = new Log(nameof(OnScriptsReloaded));
            PopupsDetailAnalyzer.AnalyzeAll(log); 
        }

        private static bool CheckPopup(out List<PopupEntryData> list)
        {
            list = new List<PopupEntryData>();

            var settingsArray = EditorCommon.FindScriptableObjects<GlobalPopupSystemSettings>();
            if (settingsArray.Length == 0)
                return false;

            if (settingsArray.Length > 1)
            {
                log.Error("If the count of 'PopupSystemSettings' is greater than 1, auto-reload will be disabled. Please ensure that only one 'PopupSystemSettings' scriptable object is present in the project.");
                return false;
            }

            var settings = settingsArray.Single();
            PopupsScanner.Scan(settings, list, log);

            return true;
        }
    }
}