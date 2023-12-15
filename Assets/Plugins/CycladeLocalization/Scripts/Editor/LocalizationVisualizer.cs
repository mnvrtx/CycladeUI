using System;
using System.Linq;
using CycladeBase.Utils;
using CycladeBaseEditor.Editor;
using CycladeLocalization;
using UnityEditor;
using UnityEngine;

namespace CycladeLocalizationEditor.Editor
{
    public class LocalizationVisualizer : EditorWindow
    {
        [MenuItem("Cyclade/Windows/" + nameof(LocalizationVisualizer))]
        public static void ShowWindow()
        {
            GetWindow(typeof(LocalizationVisualizer)); //Show existing window instance. If one doesn't exist, make one.
        }

        private Enum[] _langs;
        private readonly ClUIEditorCommon _editorCommon = new();
        private Type _areaType;

        private void OnEnable()
        {
            var langType = EnumTypeHelper.FindLang();

            _langs = Enum.GetValues(langType).Cast<Enum>().ToArray();
            titleContent.text = nameof(LocalizationVisualizer);
        }

        private void OnGUI()
        {
            if (_areaType == null) 
                _areaType = EnumTypeHelper.FindArea();

            var scene = ClUIEditorCommon.GetSceneState(out var prefabStage);

            var scTypeName = prefabStage != null ? "prefab" : "scene";
            GUILayout.Label($"Selected {scTypeName} \"{scene.name}\".", EditorStyles.largeLabel);

            var allLocalizationSetters = ClUIEditorCommon.FindAllFromScene<LocalizationSetter>(scene);
            GUILayout.Label($"Localization setters count in {scTypeName}: {allLocalizationSetters.Count}", EditorStyles.boldLabel);

            if (allLocalizationSetters.Count == 0)
                return;

            GUILayout.Label($"{string.Join(",\n", allLocalizationSetters.Select(q => $"  {q.gameObject.GetFullPath(2)} – {q.area}({q.SelectedEditorLanguage.Value})"))}", EditorStyles.label);

            EditorGUILayout.BeginHorizontal();

            for (var i = 0; i < _langs.Length; i++)
            {
                var langEnum = _langs[i];

                if (i % 5 == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                if (GUILayout.Button(langEnum.ToString(), GUILayout.Width(40)))
                {
                    var localization = new Localization();
                    localization.LoadLanguage(langEnum);

                    foreach (var setter in allLocalizationSetters)
                    {
                        setter.SelectedEditorLanguage.Value = langEnum;

                        foreach (var locInfo in setter.locInfos)
                        {
                            if (locInfo.Text == null)
                            {
                                Debug.LogError($"Set text to {locInfo.Key}. locInfo.Text == null");
                                continue;
                            }

                            Enum.TryParse(_areaType, setter.area, true, out var setterArea);
                            locInfo.Text.text = localization.GetText((Enum)setterArea, locInfo.Key);
                            EditorUtility.SetDirty(locInfo.Text);
                        }
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("$K$", GUILayout.Width(40)))
            {
                foreach (var setter in allLocalizationSetters)
                {
                    foreach (var locInfo in setter.locInfos)
                    {
                        if (locInfo.Text == null)
                        {
                            Debug.LogError($"Set text to {locInfo.Key}");
                            continue;
                        }

                        locInfo.Text.text = $"${locInfo.Key}$";
                        EditorUtility.SetDirty(locInfo.Text);
                    }
                }
            }
        }
    }
}