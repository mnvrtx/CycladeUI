using System;
using System.Collections.Generic;
using System.Linq;
using CycladeUI.Models;
using CycladeUI.Popups.System;
using CycladeUI.ScriptableObjects;
using CycladeUI.Utils;
using CycladeUI.Utils.Logging;
using UnityEditor;
using UnityEngine;

namespace CycladeUIEditor
{
    [CustomEditor(typeof(PopupSystemSettings))]
    public class PopupSystemSettingsEditor : Editor
    {
        private static readonly UiLog log = new(nameof(PopupSystemSettingsEditor));

        private readonly EditorCommon _editorCommon = new();
        private readonly Dictionary<string, PopupEntryData> _cachedAssets = new();

        [NonSerialized] private readonly List<PopupLoadEntry> _availablePopupsToAdd = new();
        [NonSerialized] private readonly List<PopupLoadEntry> _notFoundPopups = new();
        [NonSerialized] private GlobalPopupSystemSettings _currentSettings;
        [NonSerialized] private bool _resetCacheRequest;

        public override void OnInspectorGUI()
        {
            var settings = (PopupSystemSettings)target;

            serializedObject.Update();

            var property = serializedObject.GetIterator();
            bool enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                EditorGUI.BeginDisabledGroup(property.name is "m_Script" or "selectedPopupsSerialized");
                EditorGUILayout.PropertyField(property, true);
                EditorGUI.EndDisabledGroup();
            }

            if (settings.globalSettings != null)
            {
                DrawAddPopupsEditor(settings);
            }
            else
            {
                var globalSettings = EditorCommon.TryFindGlobalSettings<GlobalPopupSystemSettings>();
                if (globalSettings != null)
                {
                    serializedObject.FindProperty("globalSettings").objectReferenceValue = globalSettings;
                    ApplyModified(settings);
                }
                else
                {
                    EditorGUILayout.HelpBox("You need to create and set global settings", MessageType.Warning);
                }
            }

            if (settings.selectedPopups == null || settings.selectedPopups.Length == 0)
                UpdateModel(settings);

            ApplyModified(settings);
        }

        private void DrawAddPopupsEditor(PopupSystemSettings settings)
        {
            _editorCommon.DrawUILine(Color.gray);

            var selectedPopups = serializedObject.FindProperty("selectedPopupsSerialized");

            var e = Event.current;

            CheckSettings(settings, selectedPopups);

            EditorGUILayout.LabelField($"Selected popups ({selectedPopups.arraySize})", EditorStyles.whiteLargeLabel);

            if (selectedPopups.arraySize > 0)
                DrawSelectedPopups(settings, selectedPopups);
            else
                EditorGUILayout.HelpBox("Don't have any popups", MessageType.Warning);

            _editorCommon.DrawUILine(Color.gray);

            EditorGUILayout.LabelField($"Available popups to add ({_availablePopupsToAdd.Count})", EditorStyles.whiteLargeLabel);
            if (_availablePopupsToAdd.Count > 0)
                DrawAvailablePopups(settings, selectedPopups);
            else
                EditorGUILayout.HelpBox("All popups added", MessageType.Info);

            if (_notFoundPopups.Count > 0)
            {
                _editorCommon.DrawUILine(Color.gray);

                EditorGUILayout.HelpBox($"Popups without prefabs ({_notFoundPopups.Count})", MessageType.Warning);

                foreach (var entry in _notFoundPopups) 
                    GUILayout.Label($"\"{PopupInfo.ToShortString(entry.typeFullName)}\" prefab not found. Please add it to the Resources folder.", _editorCommon.RichLabel);
            }

            if (e.commandName == "UndoRedoPerformed")
                ScanAvailablePopups(settings);

            if (_resetCacheRequest)
            {
                ResetCache(selectedPopups);
                _resetCacheRequest = false;
            }
        }

        private string _selectedQuery = string.Empty;
        private int _querySelectedCount;

        private void DrawSelectedPopups(PopupSystemSettings settings, SerializedProperty selectedPopups)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search: ", GUILayout.Width(50));
            _selectedQuery = EditorGUILayout.TextField(_selectedQuery);
            GUILayout.EndHorizontal();
            if (!string.IsNullOrEmpty(_selectedQuery))
                EditorGUILayout.LabelField($"Found: {_querySelectedCount}/{selectedPopups.arraySize}");

            _querySelectedCount = 0;
            for (int i = 0; i < selectedPopups.arraySize; i++)
            {
                var load = PopupLoadEntry.ConvertFromString(selectedPopups.GetArrayElementAtIndex(i).stringValue);
                string labelTitle = PopupInfo.ToShortestString(load.typeFullName);

                // Construct the label with highlighted search query
                int index = labelTitle.IndexOf(_selectedQuery, StringComparison.OrdinalIgnoreCase);
                if (!string.IsNullOrEmpty(_selectedQuery) && index == -1)
                    continue;

                _querySelectedCount++;

                var beforeQuery = labelTitle.Substring(0, index);
                var query = labelTitle.Substring(index, _selectedQuery.Length);
                var afterQuery = labelTitle.Substring(index + _selectedQuery.Length);

                var displayTitle = $"{beforeQuery}{query.ColorMark()}{afterQuery}";

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(displayTitle, _editorCommon.RichLabel);
                var popupLoadType = load.type;
                load.type = (PopupLoadType)EditorGUILayout.EnumPopup(popupLoadType);
                if (load.type != popupLoadType)
                {
                    selectedPopups.GetArrayElementAtIndex(i).stringValue = load.ConvertToString();
                    ApplyModified(settings);
                }

                var assetPath = load.assetPath;
                if (!_cachedAssets.ContainsKey(assetPath))
                {
                    var type = load.TryFindType();
                    var formatPath = PopupLoader.FormatPath(assetPath);
                    var popup = Resources.Load<BasePopup>(formatPath);

                    _cachedAssets[assetPath] = new PopupEntryData(null)
                    {
                        Asset = popup,
                        Type = type,
                    };
                }

                var entryData = _cachedAssets[assetPath];
                if (entryData != null)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(entryData.Asset, entryData.Type, false);
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    _resetCacheRequest = true;
                }

                if (GUILayout.Button("Remove"))
                {
                    selectedPopups.DeleteArrayElementAtIndex(i);
                    ApplyModified(settings);
                    ScanAvailablePopups(settings);
                    break;
                }

                GUILayout.EndHorizontal();
            }
        }

        private void ApplyModified(PopupSystemSettings settings)
        {
            var applied = serializedObject.ApplyModifiedProperties();
            if (applied)
            {
                UpdateModel(settings);
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private string _availableQuery = string.Empty;
        private int _queryAvailableCount;

        private void DrawAvailablePopups(PopupSystemSettings settings, SerializedProperty selectedPopups)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search: ", GUILayout.Width(50));
            _availableQuery = EditorGUILayout.TextField(_availableQuery);
            GUILayout.EndHorizontal();
            if (!string.IsNullOrEmpty(_availableQuery))
                EditorGUILayout.LabelField($"Found: {_queryAvailableCount}/{_availablePopupsToAdd.Count}");

            _queryAvailableCount = 0;

            foreach (var entry in _availablePopupsToAdd)
            {
                var labelTitle = PopupInfo.ToShortString(entry.typeFullName);

                int index = labelTitle.IndexOf(_availableQuery, StringComparison.OrdinalIgnoreCase);

                if (!string.IsNullOrEmpty(_availableQuery) && index == -1)
                    continue;

                _queryAvailableCount++;

                var beforeQuery = labelTitle.Substring(0, index);
                var query = labelTitle.Substring(index, _availableQuery.Length);
                var afterQuery = labelTitle.Substring(index + _availableQuery.Length);

                var displayTitle = $"{beforeQuery}{query.ColorMark()}{afterQuery}";

                var buttonRect = GUILayoutUtility.GetRect(new GUIContent(displayTitle), _editorCommon.RichButton);

                //ADD
                if (GUI.Button(buttonRect, displayTitle, _editorCommon.RichButton))
                {
                    selectedPopups.arraySize++;
                    selectedPopups.GetArrayElementAtIndex(selectedPopups.arraySize - 1).stringValue = entry.ConvertToString();
                    ApplyModified(settings);
                    ScanAvailablePopups(settings);
                    break;
                }

                GUI.Label(buttonRect, "Add  ", _editorCommon.RichButtonRightAlign);
            }
        }

        private void ScanAvailablePopups(PopupSystemSettings settings)
        {
            _availablePopupsToAdd.Clear();
            _notFoundPopups.Clear();
            foreach (var entry in settings.globalSettings.entries)
            {
                if (settings.selectedPopups.Any(q => q.EqualsPopupInfo(entry.type)))
                    continue;
                if (entry.assetPath == "")
                {
                    _notFoundPopups.Add(new PopupLoadEntry(entry));
                    continue;
                }

                _availablePopupsToAdd.Add(new PopupLoadEntry(entry));
            }
        }

        private void CheckSettings(PopupSystemSettings settings, SerializedProperty selectedPopups)
        {
            if (_currentSettings != settings.globalSettings)
            {
                if (_currentSettings != null)
                    ResetCache(selectedPopups);

                PopupsScanner.Scan(settings.globalSettings, new List<PopupEntryData>(), log);
                PopupsDetailAnalyzer.AnalyzeOne(settings, log);
                ScanAvailablePopups(settings);
                _currentSettings = settings.globalSettings;
                UpdateModel(settings);
            }
        }

        private void ResetCache(SerializedProperty selectedPopups)
        {
            selectedPopups.ClearArray();
            _currentSettings = null;
            _cachedAssets.Clear();
            log.Debug("reset cache");
        }

        public void UpdateModel(PopupSystemSettings settings)
        {
            settings.FillFromSerialized();
        }
    }
}