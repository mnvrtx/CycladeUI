using System;
using System.Collections.Generic;
using System.Linq;
using CycladeBase.Utils;
using CycladeBindings;
using CycladeBindings.UIStateSystem.Base;
using CycladeBindingsEditor.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CycladeBaseEditor.Editor
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

        [SerializeField] private int selectedGroup;
        [SerializeField] private List<GroupIndexData> selectedGroupIndexes;

        private void OnGUI()
        {
            var scene = CycladeEditorCommon.GetSceneState(out var prefabStage);
            if (prefabStage == null)
                return; //todo: description
            
            _scrollView = GUILayout.BeginScrollView(_scrollView);
            
            GUILayout.Label($"Selected prefab \"{prefabStage.scene.name}\".", EditorStyles.largeLabel);

            var elementStates = CycladeEditorCommon.FindAllFromScene<BaseStatefulElement>(scene, true);
            var bindingSettings = prefabStage.prefabContentsRoot.GetComponent<BindingGenerator>();
            
            if (bindingSettings == null)
                return; //todo: description

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
                foreach (var elementState in elementStates)
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