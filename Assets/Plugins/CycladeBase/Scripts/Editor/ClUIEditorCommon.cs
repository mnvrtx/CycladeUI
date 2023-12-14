using System;
using System.Collections.Generic;
using System.Linq;
using CycladeBase.Utils;
using CycladeBase.Utils.Logging;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CycladeBaseEditor.Editor
{
    public class ClUIEditorCommon
    {
        private static readonly Log log = new(nameof(ClUIEditorCommon));

        public readonly Cache<Texture> FolderIcon = new(() => EditorGUIUtility.IconContent("d_Folder Icon").image);
        public readonly Cache<Texture> ReloadIcon = new(() => EditorGUIUtility.IconContent("TreeEditor.Refresh").image);
        public readonly Cache<Texture> CreateAddNewIcon = new(() => EditorGUIUtility.IconContent("CreateAddNew").image);
        public readonly Cache<Texture> DeleteIcon = new(() => EditorGUIUtility.IconContent("d_winbtn_win_close").image);

        public readonly Cache<GUIStyle> RichLabel = new(() =>
        {
            var s = new GUIStyle(EditorStyles.label);
            s.richText = true;
            return s;
        });

        public readonly Cache<GUIStyle> YellowLabel = new(() =>
        {
            var s = new GUIStyle(EditorStyles.label);
            s.normal.textColor = new Color(1f, 0.75f, 0f);
            s.fontSize = 12;
            s.richText = true;
            return s;
        });

        public readonly Cache<GUIStyle> RichButton = new(() =>
        {
            var s = new GUIStyle(GUI.skin.GetStyle("Button"));
            s.richText = true;
            s.alignment = TextAnchor.MiddleLeft;
            return s;
        });

        public readonly Cache<GUIStyle> RichButtonRightAlign = new(() =>
        {
            var btnStyle = new GUIStyle(GUI.skin.GetStyle("Button"));

            var style = new GUIStyle
            {
                alignment = TextAnchor.MiddleRight,
                fontSize = btnStyle.fontSize,
                normal = btnStyle.normal,
            };

            return style;
        });

        public readonly Cache<GUIStyle> RichHelp = new(() =>
        {
            var s = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
            s.richText = true;
            return s;
        });

        public readonly Cache<GUIStyle> RichFoldout = new(() =>
        {
            var s = new GUIStyle(EditorStyles.foldout);
            s.richText = true;
            return s;
        });

        public readonly Cache<GUIStyle> RichBoldLabel = new(() =>
        {
            var s = new GUIStyle(EditorStyles.boldLabel);
            s.richText = true;
            return s;
        });

        public readonly Cache<GUIStyle> ErrorText = new(() =>
        {
            var guiStyle = new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 12,
            };

            guiStyle.normal.textColor = CycladeHelpers.ParseHex("FF4251");

            return guiStyle;
        });

        public static Scene GetSceneState(out PrefabStage prefabStage)
        {
            Scene scene;
            prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage != null)
                scene = prefabStage.scene;
            else
                scene = SceneManager.GetActiveScene();
            return scene;
        }

        public static List<T> FindAllFromScene<T>(Scene scene)
        {
            var allComponentns = scene.GetRootGameObjects()
                .SelectMany(q => q.GetComponentsInChildren<T>()).ToList();
            return allComponentns;
        }


        public bool AddButton(string tooltip, int width)
        {
            return GUILayout.Button(new GUIContent(CreateAddNewIcon.Value, tooltip), GUILayout.Width(width));
        }

        public bool DeleteButton(string tooltip, int width)
        {
            return GUILayout.Button(new GUIContent(DeleteIcon.Value, tooltip), GUILayout.Width(width));
        }

        public bool RefreshButton(string tooltip, int width)
        {
            return GUILayout.Button(new GUIContent(ReloadIcon.Value, tooltip), GUILayout.Width(width));
        }

        public void SaveState(Object obj, string str)
        {
            Undo.RegisterCompleteObjectUndo(obj, str);
            Undo.FlushUndoRecordObjects();
        }

        public void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        public void DrawVerticalLine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(padding + thickness));
            r.width = thickness;
            r.x += padding / 2;
            r.y -= 2;
            r.height += 6;
            EditorGUI.DrawRect(r, color);
        }

        public T Property<T>(Func<T> f, Object target, string str)
        {
            EditorGUI.BeginChangeCheck();
            var cache = f();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, str);
            }

            return cache;
        }

        public static void OffsetBlock(Action contentRenderer, bool needBorder = false)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(10, false);

            if (needBorder)
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(false));
            else
                EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));

            contentRenderer.Invoke();
            EditorGUILayout.Space(2, false);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        public static List<T> FindObjectsType<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets("t:Prefab");
            var assets = new List<T>();

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (asset.TryGetComponent<T>(out var component))
                {
                    assets.Add(component);
                }
            }

            return assets;
        }
        
        public static T TryFindGlobalSettings<T>() where T : ScriptableObject
        {
            var settingsArray = FindScriptableObjects<T>();
            if (settingsArray.Length == 0)
                return default;

            if (settingsArray.Length > 1)
            {
                log.Error($"Please ensure that only one '{typeof(T).Name}' scriptable object is present in the project.");
                return default;
            }

            return settingsArray.Single();
        }

        public static T[] FindScriptableObjects<T>() where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            var assets = new T[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }

            return assets;
        }
    }
}