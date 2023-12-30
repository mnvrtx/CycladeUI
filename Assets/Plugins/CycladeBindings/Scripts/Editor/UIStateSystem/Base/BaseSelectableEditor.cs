using System;
using System.Linq;
using CycladeBaseEditor;
using CycladeBindings;
using CycladeBindings.UIStateSystem.Base;
using UnityEditor;
using UnityEngine;

namespace CycladeBindingsEditor.UIStateSystem.Base
{
    public abstract class BaseSelectableEditor<TState> : Editor
    {
        private SerializedProperty _stateGroupProp;
        private SerializedProperty _statesProp;
        private SerializedProperty _orderProp;

        private int _selectedGroup;

        private readonly CycladeEditorCommon _editorCommon = new();

        private bool _showAdditional;

        private void OnEnable()
        {
            _stateGroupProp = serializedObject.FindProperty("stateGroup");
            _statesProp = serializedObject.FindProperty("states");
            _orderProp = serializedObject.FindProperty("order");
        }

        protected abstract void DrawPropValues(SerializedProperty stateProp);

        public override void OnInspectorGUI()
        {
            var settings = (BaseGroupableState)target;

            var bindingSettings = settings.GetComponentInParent<BindingGenerator>();

            serializedObject.Update();


            // Popup for selecting stateGroup
            if (bindingSettings != null && bindingSettings.stateGroups != null)
            {
                var groupNames = bindingSettings.stateGroups.Select(q => q.name).ToList();

                _selectedGroup = groupNames.IndexOf(_stateGroupProp.stringValue);
                int newSelectedGroup = EditorGUILayout.Popup("State Group", _selectedGroup, groupNames.ToArray());
                if (newSelectedGroup >= 0)
                {
                    _stateGroupProp.stringValue = groupNames[newSelectedGroup];
                    serializedObject.ApplyModifiedProperties();
                }
            }

            _editorCommon.DrawUILine();

            // Managing the list of states
            if (_statesProp != null)
            {
                for (int i = 0; i < _statesProp.arraySize; i++)
                {
                    var stateProp = _statesProp.GetArrayElementAtIndex(i);
                    var nameProp = stateProp.FindPropertyRelative("state");

                    EditorGUILayout.BeginHorizontal();

                    // Popup for state
                    if (bindingSettings != null && bindingSettings.stateGroups != null && _selectedGroup >= 0)
                    {
                        var stateNames = bindingSettings.stateGroups[_selectedGroup].states.ToList();
                        stateNames.Add(BindingConstants.AnyOtherState);
                        int selectedState = stateNames.IndexOf(nameProp.stringValue);
                        int newSelectedState = EditorGUILayout.Popup(selectedState, stateNames.ToArray());
                        if (newSelectedState >= 0)
                        {
                            nameProp.stringValue = stateNames[newSelectedState];
                            serializedObject.ApplyModifiedProperties();
                        }
                    }

                    DrawPropValues(stateProp);

                    // Remove button
                    if (GUILayout.Button("Remove"))
                    {
                        _statesProp.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                    }

                    if (GUILayout.Button("Add below"))
                    {
                        _statesProp.InsertArrayElementAtIndex(i + 1);
                        _statesProp.GetArrayElementAtIndex(i + 1).boxedValue = Activator.CreateInstance<TState>();
                        serializedObject.ApplyModifiedProperties();
                    }

                    EditorGUILayout.EndHorizontal();
                }

                _editorCommon.DrawUILine();

                // Add button
                if (GUILayout.Button("Add New State"))
                {
                    _statesProp.arraySize++;
                    _statesProp.GetArrayElementAtIndex(_statesProp.arraySize - 1).boxedValue = Activator.CreateInstance<TState>();
                    serializedObject.ApplyModifiedProperties();
                }

                _editorCommon.DrawUILine();
                
                DrawExtra();

                _showAdditional = EditorGUILayout.Foldout(_showAdditional, "Additional settings");

                if (_showAdditional)
                {
                    EditorGUILayout.PropertyField(_orderProp, new GUIContent("Optional State Execution Order"));
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        public virtual void DrawExtra()
        {
            
        }
    }
}