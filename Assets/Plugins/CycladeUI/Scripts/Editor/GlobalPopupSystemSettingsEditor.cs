using System.Collections.Generic;
using CycladeBaseEditor;
using CycladeUI.Models;
using CycladeUI.Popups.System;
using CycladeUI.ScriptableObjects;
using Shared.Utils.Logging;
using UnityEditor;
using UnityEngine;

namespace CycladeUIEditor
{
    [CustomEditor(typeof(GlobalPopupSystemSettings))]
    public class GlobalPopupSystemSettingsEditor : Editor
    {
        private static readonly Log log = new(nameof(GlobalPopupSystemSettingsEditor), CycladeDebugInfo.I);

        private Vector2 _scrollPosition, _scrollPosition2, _scrollPosition3;

        [SerializeField] private bool isDebug;

        private readonly List<PopupEntryData> _foundEntryDataList = new();

        private readonly CycladeEditorCommon _editorCommon = new();

        private bool _cycladeUIDebug;

        private void OnEnable()
        {
            _cycladeUIDebug = SessionState.GetBool(CycladeDebugInfo.DebugKey, false);

            var settings = (GlobalPopupSystemSettings)target;

            foreach (var entry in settings.entries)
            {
                var foundType = PopupInfo.TryFind(entry.type.assemblyName, entry.type.fullName);
                var entryData = new PopupEntryData(entry);
                entryData.Type = foundType;
                _foundEntryDataList.Add(entryData);
            }
        }

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
            while (property.NextVisible(true))
            {
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
                SessionState.SetBool(CycladeDebugInfo.DebugKey, _cycladeUIDebug);
                CycladeDebugInfo.ResetCache();
                log.Info($"set DebugCycladeUI: {_cycladeUIDebug}");
            }
        }

        private void DrawButtons(GlobalPopupSystemSettings settings)
        {
            if (GUILayout.Button("Rescan Popups")) 
                PopupsScanner.ScanAndSaveToSettings(settings, _foundEntryDataList, log);

            EditorGUILayout.HelpBox("Rescanning occurs automatically either when the project is recompiled or when a new popup is added.", MessageType.Info);
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
                        log.Warn($"The asset of type '{data.Entry.type.ToShortString()}' was not found at the specified path: '{data.Entry.assetPath}'. It may have been renamed, moved, or deleted.");
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