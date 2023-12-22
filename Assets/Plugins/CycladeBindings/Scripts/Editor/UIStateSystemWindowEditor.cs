using System;
using System.Collections.Generic;
using System.Linq;
using CycladeBase.Utils;
using CycladeBaseEditor;
using CycladeBindings;
using CycladeBindings.UIStateSystem.Base;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CycladeBindingsEditor
{
    public class UIStateSystemWindowEditor : EditorWindow
    {
        [Serializable]
        public class GroupIndexData
        {
            public int index;
            public string name;

            public GroupIndexData(int index, string name)
            {
                this.index = index;
                this.name = name;
            }
        }

        [MenuItem("Cyclade/Windows/" + nameof(UIStateSystemWindowEditor))]
        public static void ShowWindow() => GetWindow<UIStateSystemWindowEditor>(nameof(UIStateSystemWindowEditor));

        private readonly CycladeEditorCommon _editorCommon = new();
        private Vector2 _scrollView = Vector2.zero;
        
        [NonSerialized] private Scene _sceneCache;
        [NonSerialized] private object _bindComponent;
        [NonSerialized] private Type _bindingType;
        [NonSerialized] private readonly HashSet<BaseStatefulElement> _statefulElementsInBindings = new();

        [SerializeField] private List<GroupIndexData> selectedGroupIndexes;
        
        private readonly List<BaseStatefulElement> _foundStatefulElements = new();
        private readonly List<BaseStatefulElement> _statefulElementsCache = new();

        private void OnGUI()
        {
            var scene = CycladeEditorCommon.GetSceneState(out var prefabStage);
            if (prefabStage == null)
            {
                EditorGUILayout.HelpBox($"Testing states is only possible in an open prefab, and the prefab must not be a prefab variant.", MessageType.Info);
                return;
            }
            
            GUILayout.Label($"Selected prefab \"{prefabStage.scene.name}\".", EditorStyles.largeLabel);

            var bindingSettings = prefabStage.prefabContentsRoot.GetComponent<BindingGenerator>();

            if (bindingSettings == null)
            {
                EditorGUILayout.HelpBox($"Not found binding settings", MessageType.Warning);
                return;
            }

            _foundStatefulElements.Clear();
            CycladeHelpers.IterateOverMeAndChildren(bindingSettings.gameObject, go =>
            {
                go.GetComponents(_statefulElementsCache);
                if (_statefulElementsCache.Count == 0)
                    return;

                _foundStatefulElements.AddRange(_statefulElementsCache);
            }, o => !PrefabUtility.IsAnyPrefabInstanceRoot(o));
            
            if (_foundStatefulElements.Count == 0)
            {
                EditorGUILayout.HelpBox($"Not found stateful elements", MessageType.Info);
                return;
            }

            if (_sceneCache != scene)
            {
                BindingGeneratorEditor.TryFindType(bindingSettings, out _bindingType);
                _bindComponent = bindingSettings.GetComponent(_bindingType);
                _sceneCache = scene;
            }

            {
                var statefulElements = _bindComponent.GetPrivateOrOtherField<List<BaseStatefulElement>>("statefulElements");
                if (statefulElements != null)
                {
                    _statefulElementsInBindings.Clear();
                    foreach (var el in statefulElements)
                    {
                        if (el == null)
                            continue;
                        _statefulElementsInBindings.Add(el);
                    }

                    foreach (var statefulElement in _foundStatefulElements)
                    {
                        if (_statefulElementsInBindings.Contains(statefulElement))
                            continue;

                        GUILayout.Label($"<color=red>Not found {statefulElement.GetType().Name} statefulElement in binding.</color> Please, press \"Update bindings\".", _editorCommon.RichBoldLabel);
                    }    
                }
            }

            _scrollView = GUILayout.BeginScrollView(_scrollView);


            bool changed = false;
            
            for (var i = 0; i < bindingSettings.stateGroups.Count; i++)
            {
                var stateGroup = bindingSettings.stateGroups[i];
                GUILayout.Label($"Group {stateGroup.name}");
                var stateNames = stateGroup.states;
                var selectedState = selectedGroupIndexes.TryGet(i)?.index ?? 0;

                // Iterate through each state and create a button
                for (int j = 0; j < stateNames.Count; j++)
                {
                    var stateName = selectedState == j ? $"<color=orange><b>{stateNames[j]}</b></color>" : stateNames[j];
                    if (GUILayout.Button(stateName, _editorCommon.RichButton))
                    {
                        selectedGroupIndexes.Set(i, new GroupIndexData(j, stateGroup.name));
                        if (selectedState != j)
                        {
                            changed = true;
                        }
                        break; // Break the loop if a button is pressed
                    }
                }
                
                _editorCommon.DrawUILine();
            }

            if (GUILayout.Button("Force Re-apply state") || changed)
            {
                foreach (var elementState in _foundStatefulElements)
                {
                    if (elementState is BaseActivatableState baseActivatableState)
                    {
                        var stateGroup = baseActivatableState.stateGroup;
                        var selectedInfo = selectedGroupIndexes.FirstOrDefault(q => q.name == stateGroup);
                        if (selectedInfo != null)
                        {
                            var statesGroup = bindingSettings.stateGroups.FirstOrDefault(q => q.name == stateGroup);
                            if (statesGroup != null)
                                baseActivatableState.Select(statesGroup.states.TryGet(selectedInfo.index));
                        }
                    }
                    else if (elementState is BaseSelectableState selectable)
                    {
                        var stateGroup = selectable.stateGroup;
                        var selectedInfo = selectedGroupIndexes.FirstOrDefault(q => q.name == stateGroup);
                        if (selectedInfo != null)
                        {
                            var statesGroup = bindingSettings.stateGroups.FirstOrDefault(q => q.name == stateGroup);
                            if (statesGroup != null)
                                selectable.Select(statesGroup.states.TryGet(selectedInfo.index));
                        }
                    }
                }
                SceneView.RepaintAll(); 
            }

            GUILayout.EndScrollView();
        }
    }
}