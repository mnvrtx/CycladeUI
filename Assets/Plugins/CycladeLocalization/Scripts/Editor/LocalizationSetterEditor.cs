using System;
using CycladeBaseEditor;
using CycladeLocalization;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace CycladeLocalizationEditor
{
    [CustomEditor(typeof(LocalizationSetter))]
    [CanEditMultipleObjects]
    public class LocalizationSetterEditor : Editor
    {
        private readonly CycladeEditorCommon _editorCommon = new();

        private Localization _localization;
        private string[] _keys;

        private Enum _currentLang;
        
        private Type _areaType;
        private Type _langType;

        private void OnEnable()
        {
            UpdateTypes();

            var values = Enum.GetValues(_langType);
            var firstValue = (Enum)values.GetValue(0);

            SetLoc(firstValue);
        }

        private void SetLoc(Enum lang)
        {
            _currentLang = lang;

            _localization = new Localization();
            _localization.LoadLanguage(lang);
        }

        private void UpdateKeys(LocalizationSetter setter)
        {
            var values = Enum.GetValues(_areaType);
            var firstValue = (Enum)values.GetValue(0);
            
            if (string.IsNullOrEmpty(setter.area))
                setter.area = firstValue.ToString();
            _keys = _localization.GetKeys((Enum)Enum.Parse(_areaType, setter.area));
        }

        public void UpdateTypes()
        {
            if (_langType == null)
                _langType = EnumTypeHelper.FindLang();

            if (_areaType == null)
                _areaType = EnumTypeHelper.FindArea();
        }

        public override void OnInspectorGUI()
        {
            if (_localization.IsNotLoaded)
            {
                EditorGUILayout.HelpBox($"Localization is not loaded. Please press {GoogleSheetsSyncManager.UpdateCommandFullName}", MessageType.Warning);
                return;
            }
            var setter = target as LocalizationSetter;
            if (setter == null)
                return;

            UpdateTypes();

            if (_keys == null)
                UpdateKeys(setter);

            EditorGUILayout.BeginHorizontal();

            if (_editorCommon.RefreshButton("Refresh keys from selected area", 40)) {
                SetLoc(_currentLang);
                UpdateKeys(setter);
            }

            if (setter.SelectedEditorLanguage.HasValue) {
                if (_currentLang != setter.SelectedEditorLanguage.Value) {
                    SetLoc(setter.SelectedEditorLanguage.Value);
                    UpdateKeys(setter);
                }    
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(15);
        
            Enum.TryParse(_areaType, setter.area, true, out var areaRaw);
            var area = (Enum)areaRaw;
            var beforeArea = area;
            area = EditorGUILayout.EnumPopup(area);

            if (!Equals(beforeArea, area)) {
                setter.area = area.ToString();
                EditorUtility.SetDirty(target);
                UpdateKeys(setter);
            }

            if (_editorCommon.AddButton("Add entry", 40))
            {
                _editorCommon.SaveState(setter, "Added localization setter entry");
                setter.locInfos.Add(new LocInfoKvp());
                EditorUtility.SetDirty(target);
            }

            if (setter.locInfos == null)
                return;

            _editorCommon.DrawUILine(Color.gray);
            for (var i = 0; i < setter.locInfos.Count; i++)
            {
                var entry = setter.locInfos[i];
            
                EditorGUILayout.BeginHorizontal();
                entry.Text = _editorCommon.Property(() => (TMP_Text) EditorGUILayout.ObjectField(entry.Text, typeof(TMP_Text), true),
                    setter, $"Changed localization text {i}");

                var keyBefore = entry.Key;
                entry.Key = _editorCommon.Property(() => EditorGUILayout.TextField(entry.Key),
                    setter, $"Changed localization idx {i}");

                if (keyBefore != entry.Key) {
                    entry.KeyIndex = -1;
                    EditorUtility.SetDirty(target);
                }

                if (_editorCommon.DeleteButton("Remove entry", 40))
                {
                    _editorCommon.SaveState(setter, "Removed localization entry");
                    setter.locInfos.RemoveAt(i);
                    EditorUtility.SetDirty(target);
                    i--;
                }
                EditorGUILayout.EndHorizontal();


                if (entry.KeyIndex == -1)
                    entry.KeyIndex = Array.IndexOf(_keys, entry.Key);

                var keyIndexBefore = entry.KeyIndex;
                entry.KeyIndex = EditorGUILayout.Popup("", entry.KeyIndex, _keys);

                if (keyIndexBefore != entry.KeyIndex)
                    EditorUtility.SetDirty(target);

                if (entry.KeyIndex != -1) {
                    if (entry.KeyIndex >= _keys.Length)
                        entry.KeyIndex = 0;
                    entry.Key = _keys[entry.KeyIndex];
                }

                EditorGUILayout.BeginHorizontal();
                string text;

                text = entry.Text != null ? entry.Text.text : "";
                EditorGUILayout.LabelField($"View: {text.Replace("\n", "")}");
            
                text = _localization.GetText(area, entry.Key, false);
                EditorGUILayout.LabelField($"KeyValue: {text.Replace("\n", "")}");

                EditorGUILayout.EndHorizontal();
            
                _editorCommon.DrawUILine(Color.gray);
                EditorGUILayout.Space(20);
            
            }
        }
    }
}