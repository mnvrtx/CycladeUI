using System;
using System.Collections.Generic;
using System.Linq;
using CycladeBaseEditor;
using CycladeBindings;
using CycladeBindings.UIStateSystem.Base;
using DG.DOTweenEditor;
using DG.Tweening;
using Shared.Utils;
using Solonity.View.Utils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CycladeBindingsEditor.Windows
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

        [MenuItem("Window/Cyclade/UIStateSystem")]
        public static void ShowWindow() => GetWindow<UIStateSystemWindowEditor>("UIStateSystem");

        private readonly CycladeEditorCommon _editorCommon = new();
        private Vector2 _scrollView = Vector2.zero;

        private bool _isEnabledAnimations;
        
        [NonSerialized] private Scene _sceneCache;
        [NonSerialized] private object _bindComponent;
        [NonSerialized] private Type _bindingType;
        [NonSerialized] private readonly HashSet<BaseStatefulElement> _statefulElementsInBindings = new();

        [SerializeField] private List<GroupIndexData> selectedGroupIndexes;
        
        private readonly List<BaseStatefulElement> _foundStatefulElements = new();
        private readonly List<BaseStatefulElement> _statefulElementsCache = new();
        
        private void OnEnable()
        {
            PrefabStage.prefabStageOpened += OnPrefabOpened;
        }

        private void OnDisable()
        {
            PrefabStage.prefabStageOpened -= OnPrefabOpened;
        }

        private void OnFocus()
        {
            Setup();
        }

        private void OnPrefabOpened(PrefabStage stage)
        {
            CleanState();
            Setup();
        }

        private void Setup()
        {
            var scene = CycladeEditorCommon.GetSceneState(out var prefabStage);
            if (prefabStage == null)
                return;

            var bindingSettings = prefabStage.prefabContentsRoot.GetComponent<BindingGenerator>();
            if (bindingSettings != null)
            {
                BindingGeneratorEditor.TryFindType(bindingSettings, out _bindingType);
                _bindComponent = _bindingType != null ? bindingSettings.GetComponent(_bindingType) : null;    
            }
            
            _sceneCache = scene;
        }

        public void CleanState()
        {
            _scrollView = Vector2.zero;

            _isEnabledAnimations = false;

            _sceneCache = default;
            _bindComponent = default;
            _bindingType = default;
            _statefulElementsInBindings.Clear();

            selectedGroupIndexes = new List<GroupIndexData>();

            _foundStatefulElements.Clear();
            _statefulElementsCache.Clear();

            SceneView.RepaintAll();
            DOTweenPreviewManager.StopAllPreviews();
        }

        private void OnGUI()
        {
            // Debug.Log(Event.current.type);
            
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
                EditorGUILayout.HelpBox($"Not found binding settings", MessageType.Info);
                return;
            }

            if (_bindComponent == null)
            {
                EditorGUILayout.HelpBox($"Not found binding type", MessageType.Warning);

                if (GUILayout.Button("Try to find a type again", _editorCommon.RichButton)) 
                    Setup();

                return;
            }

            _foundStatefulElements.Clear();
            ViewUtils.IterateOverMeAndChildren(bindingSettings.gameObject, go =>
            {
                go.GetComponents(_statefulElementsCache);
                if (_statefulElementsCache.Count == 0)
                    return;

                _foundStatefulElements.AddRange(_statefulElementsCache);
            }, NeedToGoDeepFunc);
            
            if (_foundStatefulElements.Count == 0)
            {
                EditorGUILayout.HelpBox($"Not found stateful elements", MessageType.Info);
                return;
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

            _isEnabledAnimations = GUILayout.Toggle(_isEnabledAnimations, "IsEnabledAnimations");
            
            if (DOTweenPreviewManager.IsPreviewing || _isEnabledAnimations)
            {
                EditorGUILayout.HelpBox($"Saving in this state may corrupt your data. Please use preview animations only on saved prefabs.", MessageType.Warning);

                if (DOTweenPreviewManager.IsPreviewing && GUILayout.Button("StopAnimations"))
                {
                    DOTweenPreviewManager.StopAllPreviews();
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

                if (selectedState != 0) 
                    EditorGUILayout.TextArea($"If you wish to save a prefab, it is recommended to use the first state: <b>{stateNames[0]}</b>", _editorCommon.RichHelp);

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

            if (GUILayout.Button("Force Re-apply state (without anim)") || changed)
            {
                Selection.activeGameObject = null; // Clears the active selection
                Selection.objects = Array.Empty<Object>(); // Clears the entire selection

                var tweenAnimations = new List<DOTweenAnimation>();

                foreach (var elementState in _foundStatefulElements)
                {
                    if (elementState is BaseActivatableState baseActivatableState)
                    {
                        var stateGroup = baseActivatableState.stateGroup;
                        var selectedInfo = selectedGroupIndexes.FirstOrDefault(q => q?.name == stateGroup);
                        if (selectedInfo != null)
                        {
                            var statesGroup = bindingSettings.stateGroups.FirstOrDefault(q => q.name == stateGroup);
                            if (statesGroup != null)
                            {
                                var gameObject = baseActivatableState.gameObject;
                                var activeBefore = gameObject.activeSelf; 
                                baseActivatableState.Select(statesGroup.states.TryGet(selectedInfo.index));
                                if (gameObject.activeSelf && activeBefore != gameObject.activeSelf) 
                                    RegisterToPlayDoTween(gameObject, tweenAnimations);
                            }
                        }
                    }
                    else if (elementState is BaseGroupableState selectable)
                    {
                        var stateGroup = selectable.stateGroup;
                        var selectedInfo = selectedGroupIndexes.FirstOrDefault(q => q?.name == stateGroup);
                        if (selectedInfo != null)
                        {
                            var statesGroup = bindingSettings.stateGroups.FirstOrDefault(q => q.name == stateGroup);
                            if (statesGroup != null)
                            {
                                var gameObject = selectable.gameObject;
                                var activeBefore = gameObject.activeSelf; 
                                selectable.Select(statesGroup.states.TryGet(selectedInfo.index));
                                if (gameObject.activeSelf && activeBefore != gameObject.activeSelf) 
                                    RegisterToPlayDoTween(gameObject, tweenAnimations);
                            }
                        }
                    }
                }

                if (changed && tweenAnimations.Count > 0 && _isEnabledAnimations)
                {
                    Debug.Log($"PlayDOTween: [{string.Join(", ", tweenAnimations.Select(q => q.name))}]");
                    DOTweenPreviewManager.PlayAnimations(tweenAnimations.ToArray());    
                }
                
                EditorUtility.SetDirty(prefabStage.prefabContentsRoot);
                SceneView.RepaintAll(); 
            }

            GUILayout.EndScrollView();
        }
        
        private static bool NeedToGoDeepFunc(GameObject o) => !PrefabUtility.IsAnyPrefabInstanceRoot(o) || o.GetComponent<BindingGenerator>() == null;

        private void RegisterToPlayDoTween(GameObject obj, List<DOTweenAnimation> list)
        {
            obj.IterateOnMeAndChildren<Transform>(t =>
            {
                var doTweens = t.GetComponents<DOTweenAnimation>();
                list.AddRange(doTweens);
            });
        }
    }
}