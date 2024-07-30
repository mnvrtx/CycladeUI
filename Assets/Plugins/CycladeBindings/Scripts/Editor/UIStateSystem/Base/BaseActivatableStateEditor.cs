using System;
using System.Linq;
using CycladeBase.Utils;
using CycladeBindings;
using CycladeBindings.UIStateSystem.Base;
using Shared.Utils;
using UnityEditor;
using UnityEngine;

namespace CycladeBindingsEditor.UIStateSystem.Base
{
    public abstract class BaseActivatableStateEditor : Editor
    {
        protected abstract string GroupSelectorTitle { get; }
        protected abstract string StateSelectorTitle { get; }
        protected virtual string SelectorPostTitle { get; }

        private SerializedProperty _stateGroupProperty;
        private SerializedProperty _statesProperty;

        private int _lastSelectedGroupIndex = -2;
        private int _selectedGroupIndex = -2;
        private bool[] _selectedStates;
        
        private void OnEnable()
        {
            _stateGroupProperty = serializedObject.FindProperty("stateGroup");
            _statesProperty = serializedObject.FindProperty("states");
        }

        public override void OnInspectorGUI()
        {
            var settings = (BaseActivatableState)target;
            
            var bindingSettings = settings.transform.parent.GetComponentInParent<BindingGenerator>();

            if (bindingSettings == null)
            {
                EditorGUILayout.HelpBox("Binding settings not found.", MessageType.Warning);
                return;
            }
            
            DrawSelectors(bindingSettings);

            serializedObject.Update();

            var property = serializedObject.GetIterator();
            var enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (property.name is "stateGroup" or "states")
                    continue;

                EditorGUI.BeginDisabledGroup(property.name is "m_Script");
                EditorGUILayout.PropertyField(property, true);
                EditorGUI.EndDisabledGroup();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
        public void DrawSelectors(BindingGenerator bindingSettings)
        {
            string[] groupNames = bindingSettings.stateGroups.Select(q => q.name).ToArray();

            if (_selectedGroupIndex == -2)
            {
                _selectedGroupIndex = groupNames.ToList().IndexOf(_stateGroupProperty.stringValue);
                _lastSelectedGroupIndex = _selectedGroupIndex;
            }
            
            // Dropdown for State Groups
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(GroupSelectorTitle, GUILayout.Width(300));
            _selectedGroupIndex = EditorGUILayout.Popup(_selectedGroupIndex, groupNames);
            if (_lastSelectedGroupIndex != _selectedGroupIndex)
            {
                _selectedStates = null;
                _statesProperty.ClearArray();
                serializedObject.ApplyModifiedProperties();
                _lastSelectedGroupIndex = _selectedGroupIndex;
            }
            EditorGUILayout.EndHorizontal();

            // Update stateGroup property
            if(!string.IsNullOrEmpty(StateSelectorTitle) && groupNames.IsIndexValid(_selectedGroupIndex))
            {
                _stateGroupProperty.stringValue = groupNames[_selectedGroupIndex];
                serializedObject.ApplyModifiedProperties();
                
                var statesList = bindingSettings.stateGroups[_selectedGroupIndex].states;
                string[] states = statesList.ToArray();
                // EditorGUILayout.BeginHorizontal();
                GUILayout.Label(StateSelectorTitle, GUILayout.Width(300));

                if (_selectedStates == null)
                {
                    CodeHelpers.EnsureCapacity(ref _selectedStates, states.Length);
                    for (int i = 0; i < states.Length; i++)
                    {
                        string state = states[i];
                        for (int j = 0; j < _statesProperty.arraySize; j++)
                        {
                            if (_statesProperty.GetArrayElementAtIndex(j).stringValue == state)
                            {
                                _selectedStates[i] = true;
                                break;
                            }
                        }
                    }
                }
                
                
                // Multi-select popup
                EditorGUI.BeginChangeCheck();
                for (int i = 0; i < states.Length; i++) 
                    _selectedStates[i] = EditorGUILayout.ToggleLeft(states[i], _selectedStates[i]);

                if (EditorGUI.EndChangeCheck())
                {
                    // Clear the current array
                    _statesProperty.ClearArray();

                    // Add selected states to the SerializedProperty
                    for (int i = 0; i < states.Length; i++)
                    {
                        if (_selectedStates[i])
                        {
                            _statesProperty.InsertArrayElementAtIndex(_statesProperty.arraySize);
                            _statesProperty.GetArrayElementAtIndex(_statesProperty.arraySize - 1).stringValue = states[i];
                        }
                    }
                }

                serializedObject.ApplyModifiedProperties();
                
                // EditorGUILayout.EndHorizontal();
                if (!string.IsNullOrEmpty(SelectorPostTitle)) 
                    GUILayout.Label(SelectorPostTitle, GUILayout.Width(300));
            }
        }
    }
}