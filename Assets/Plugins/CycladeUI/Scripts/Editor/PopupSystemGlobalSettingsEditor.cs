using System;
using System.Collections.Generic;
using System.Diagnostics;
using CycladeUI.Popups.System;
using CycladeUI.ScriptableObjects;
using CycladeUI.Utils.Logging;
using UnityEditor;
using UnityEngine;

namespace CycladeUIEditor
{
    [CustomEditor(typeof(GlobalPopupSystemSettings))]
    public class PopupSystemGlobalSettingsEditor : Editor
    {
        private static readonly UiLog log = new(nameof(PopupSystemGlobalSettingsEditor));

        private Vector2 _scrollPosition, _scrollPosition2, _scrollPosition3;
        [NonSerialized] private bool _scanned;

        [SerializeField] private bool isDebug;

        private readonly List<PopupEntryData> _foundEntryDataList = new();

        private readonly EditorCommon _editorCommon = new();

        private bool _cycladeUIDebug;

        public override void OnInspectorGUI()
        {
            isDebug = GUILayout.Toggle(isDebug, "Debug view");
            
            if (isDebug)
            {
                _editorCommon.DrawUILine(Color.gray);
                GUILayout.Label($"Raw view", EditorStyles.whiteLargeLabel);
                DrawDefaultInspector();
                _editorCommon.DrawUILine(Color.gray);
            }
            else
            {
                var settings = (GlobalPopupSystemSettings)target;
                
                _editorCommon.DrawUILine(Color.gray);
                DrawProperties();
                _editorCommon.DrawUILine(Color.gray);

                DrawButtons(settings);
                _editorCommon.DrawUILine(Color.gray);
            
                DrawFoundAssemblies(settings);
                _editorCommon.DrawUILine(Color.gray);
            
                DrawFoundPopups();

                if (GUI.changed) 
                    EditorUtility.SetDirty(settings);

                serializedObject.ApplyModifiedProperties();    
            }
        }

        public void DrawProperties()
        {
            EditorGUILayout.HelpBox("Don't forget to press 'Save' before committing changes.", MessageType.Info);

            serializedObject.Update();

            var property = serializedObject.GetIterator();
            bool enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (property.name != "debugSafeAreaSettings")
                    continue;

                var changed = EditorGUILayout.PropertyField(property, true);
                if (changed)
                {
                    EditorUtility.SetDirty((GlobalPopupSystemSettings)target);
                    serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            var newCycladeUIDebug = EditorGUILayout.Toggle("Debug CycladeUI", _cycladeUIDebug);
            if (newCycladeUIDebug != _cycladeUIDebug)
            {
                _cycladeUIDebug = newCycladeUIDebug;
                SessionState.SetBool(UiLog.DebugKey, _cycladeUIDebug);
                UiLog.IsDebug.ResetCache();
                log.Info($"set DebugCycladeUI: {_cycladeUIDebug}");
            }
        }

        private void DrawButtons(GlobalPopupSystemSettings settings)
        {
            if (GUILayout.Button("Rescan Popups") || !_scanned)
            {
                var sw = Stopwatch.StartNew();
                PopupsScanner.Scan(settings, _foundEntryDataList, log);
                log.Debug($"RescanPopups{(!_scanned ? "(auto)" : "(manual)")}. Found {settings.assemblies.Count} assemblies with {_foundEntryDataList.Count} popups. Elapsed: {sw.ElapsedMilliseconds}ms", _scanned);
                _cycladeUIDebug = SessionState.GetBool(UiLog.DebugKey, false);
                _scanned = true;
            }

            EditorGUILayout.HelpBox("Rescanning occurs automatically either when you open this inspector or when the project is recompiled.", MessageType.Info);
        }

        private void DrawFoundAssemblies(GlobalPopupSystemSettings settings)
        {
            GUILayout.Label($"Found assemblies with popups ({settings.assemblies.Count}):", EditorStyles.whiteLargeLabel);

            if (settings.assemblies.Count == 0)
            {
                EditorGUILayout.HelpBox("Not found.", MessageType.Warning);
            }
            else
            {
                _scrollPosition2 = EditorGUILayout.BeginScrollView(_scrollPosition2, GUILayout.Height(100));
                foreach (var assembly in settings.assemblies)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Assembly - <b>{assembly}</b>", _editorCommon.RichLabel);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawFoundPopups()
        {
            GUILayout.Label($"Found popups ({_foundEntryDataList.Count}):", EditorStyles.whiteLargeLabel);

            _scrollPosition3 = EditorGUILayout.BeginScrollView(_scrollPosition3, GUILayout.Height(200));
            foreach (var data in _foundEntryDataList)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(data.Entry.type.ToShortString(), _editorCommon.RichLabel, GUILayout.Width(320));

                if ((data.Asset == null || AssetDatabase.GetAssetPath(data.Asset) != data.Entry.assetPath) && !string.IsNullOrEmpty(data.Entry.assetPath))
                {
                    data.Asset = AssetDatabase.LoadAssetAtPath<BasePopup>(data.Entry.assetPath);
                    if (data.Asset == null)
                    {
                        log.Debug($"The asset of type '{data.Entry.type.ToShortString()}' was not found at the specified path: '{data.Entry.assetPath}'. It may have been renamed, moved, or deleted. Executing an automatic re-scan.");
                        _scanned = false;
                    }
                }

                EditorGUI.BeginDisabledGroup(true);

                if (data.Asset == null)
                    GUILayout.Label($"Prefab not found. Please add it to the Resources folder.", _editorCommon.YellowLabel);
                else
                    EditorGUILayout.ObjectField(data.Asset, data.Type, false);

                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}