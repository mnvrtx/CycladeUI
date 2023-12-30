using System.Linq;
using CycladeBase.Utils;
using CycladeBindings;
using CycladeBindings.UIStateSystem.Base;
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
        private SerializedProperty _stateProperty;

        private int _selectedGroupIndex = -2;
        private int _selectedStateIndex = -2;
        
        private void OnEnable()
        {
            _stateGroupProperty = serializedObject.FindProperty("stateGroup");
            _stateProperty = serializedObject.FindProperty("state");
        }

        public override void OnInspectorGUI()
        {
            var settings = (BaseActivatableState)target;
            
            var bindingSettings = settings.GetComponentInParent<BindingGenerator>();
            DrawSelectors(bindingSettings);

            serializedObject.Update();

            var property = serializedObject.GetIterator();
            var enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (property.name is "stateGroup" or "state")
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
                _selectedGroupIndex = groupNames.ToList().IndexOf(_stateGroupProperty.stringValue);
            
            // Dropdown for State Groups
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(GroupSelectorTitle, GUILayout.Width(300));
            _selectedGroupIndex = EditorGUILayout.Popup(_selectedGroupIndex, groupNames);
            EditorGUILayout.EndHorizontal();

            // Update stateGroup property
            if(!string.IsNullOrEmpty(StateSelectorTitle) && groupNames.ValidIndex(_selectedGroupIndex))
            {
                _stateGroupProperty.stringValue = groupNames[_selectedGroupIndex];
                serializedObject.ApplyModifiedProperties();
                
                var statesList = bindingSettings.stateGroups[_selectedGroupIndex].states;
                string[] states = statesList.ToArray();
                if (_selectedStateIndex == -2) 
                    _selectedStateIndex = statesList.IndexOf(_stateProperty.stringValue);
                
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(StateSelectorTitle, GUILayout.Width(300));
                _selectedStateIndex = EditorGUILayout.Popup(_selectedStateIndex, states);
                EditorGUILayout.EndHorizontal();
                if (!string.IsNullOrEmpty(SelectorPostTitle)) 
                    GUILayout.Label(SelectorPostTitle, GUILayout.Width(300));

                // Update state property
                if(states.ValidIndex(_selectedStateIndex))
                {
                    _stateProperty.stringValue = states[_selectedStateIndex];
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}