using System.Collections.Generic;
using CycladeBaseEditor;
using CycladeBindings;
using CycladeBindings.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace CycladeBindingsEditor.Windows
{
    public class BindingsPrefabsTool : EditorWindow
    {
        private List<GameObject> bindingGenerators;
        private Vector2 scrollPosition;
        private bool showBindings = true;

        [MenuItem("Window/Cyclade/BindingsPrefabsTool")]
        public static void ShowWindow()
        {
            GetWindow<BindingsPrefabsTool>("BindingsPrefabsTool");
        }

        private void OnEnable()
        {
            FindBindingGenerators();
            UpdateTitle();
        }

        private void OnGUI()
        {
            GUILayout.Label("List of Prefabs with BindingGenerator", EditorStyles.boldLabel);

            if (bindingGenerators == null || bindingGenerators.Count == 0)
            {
                GUILayout.Label("No Binding Generators found.");
                if (GUILayout.Button("Refresh"))
                {
                    FindBindingGenerators();
                    UpdateTitle();
                }
                return;
            }

            showBindings = EditorGUILayout.Foldout(showBindings, $"Binding Generators ({bindingGenerators.Count})");
            if (showBindings)
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(500));

                foreach (var prefab in bindingGenerators)
                {
                    if (GUILayout.Button(prefab.name))
                    {
                        Selection.activeObject = prefab;
                    }
                }

                GUILayout.EndScrollView();
            }

            if (GUILayout.Button("Refresh"))
            {
                FindBindingGenerators();
                UpdateTitle();
            }

            if (GUILayout.Button("Process and regenerate all"))
            {
                ProcessAllBindingGenerators();
            }
        }

        private void FindBindingGenerators()
        {
            bindingGenerators = new List<GameObject>();
            string[] guids = AssetDatabase.FindAssets("t:Prefab");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab.GetComponent<BindingGenerator>() != null && !PrefabUtility.IsPartOfVariantPrefab(prefab))
                {
                    bindingGenerators.Add(prefab);
                }
            }
        }

        private void UpdateTitle()
        {
            titleContent = new GUIContent($"BindingsPrefabsTool ({bindingGenerators.Count})");
        }

        private void ProcessAllBindingGenerators()
        {
            foreach (var prefab in bindingGenerators)
            {
                string path = AssetDatabase.GetAssetPath(prefab);
                GameObject instance = PrefabUtility.LoadPrefabContents(path);
                try
                {
                    var bindingGeneratorComponent = instance.GetComponent<BindingGenerator>();
                    if (bindingGeneratorComponent != null)
                    {
                        var globalSettings = CycladeEditorHelpers.TryFindGlobalSettings<GlobalCycladeBindingsSettings>();
                        BindingGeneratorEditor.Generate(bindingGeneratorComponent, null, globalSettings, true, false, false);

                        PrefabUtility.SaveAsPrefabAsset(instance, path);
                    }
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(instance);
                }
            }
            AssetDatabase.Refresh();
        }
    }
}