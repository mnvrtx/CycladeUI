using System.Linq;
using CycladeBaseEditor;
using CycladeBindings;
using CycladeBindings.UIStateSystem.Elements;
using UnityEditor;
using UnityEngine;

namespace CycladeBindingsEditor.UIStateSystem
{
    [CustomEditor(typeof(SelectableRectTransform))]
    public class SelectableRectTransformEditor : UnityEditor.Editor
    {
        SerializedProperty stateGroupProp;
        SerializedProperty statesProp;

        private int selectedGroup;
        
        private readonly CycladeEditorCommon _editorCommon = new();

        void OnEnable()
        {
            // Cache the SerializedProperties
            stateGroupProp = serializedObject.FindProperty("stateGroup");
            statesProp = serializedObject.FindProperty("states");
        }

        public override void OnInspectorGUI()
        {
            var settings = (SelectableRectTransform)target;
            
            var bindingSettings = settings.GetComponentInParent<BindingGenerator>();
            
            serializedObject.Update();

            // Popup for selecting stateGroup
            if (bindingSettings != null && bindingSettings.stateGroups != null)
            {
                var groupNames = bindingSettings.stateGroups.Select(q => q.name).ToList();

                selectedGroup = groupNames.IndexOf(stateGroupProp.stringValue);
                int newSelectedGroup = EditorGUILayout.Popup("State Group", selectedGroup, groupNames.ToArray());
                if (newSelectedGroup >= 0)
                {
                    stateGroupProp.stringValue = groupNames[newSelectedGroup];
                    serializedObject.ApplyModifiedProperties();
                }
            }
            
            _editorCommon.DrawUILine();

            // Managing the list of states
            if (statesProp != null)
            {
                for (int i = 0; i < statesProp.arraySize; i++)
                {
                    SerializedProperty stateProp = statesProp.GetArrayElementAtIndex(i);
                    SerializedProperty nameProp = stateProp.FindPropertyRelative("state");
                    SerializedProperty rectTransformProp = stateProp.FindPropertyRelative("rectTransform");
                    
                    EditorGUILayout.BeginHorizontal();

                    // Popup for state
                    if (bindingSettings != null && bindingSettings.stateGroups != null && selectedGroup >= 0)
                    {
                        var stateNames = bindingSettings.stateGroups[selectedGroup].states.ToList();
                        stateNames.Add(BindingConstants.AnyOtherState);
                        int selectedState = stateNames.IndexOf(nameProp.stringValue);
                        int newSelectedState = EditorGUILayout.Popup(selectedState, stateNames.ToArray());
                        if (newSelectedState >= 0)
                        {
                            nameProp.stringValue = stateNames[newSelectedState];
                            serializedObject.ApplyModifiedProperties();
                        }
                    }

                    var before = rectTransformProp.objectReferenceValue;
                    rectTransformProp.objectReferenceValue = EditorGUILayout.ObjectField(before, typeof(RectTransform), allowSceneObjects: true);
                    if (before != rectTransformProp.objectReferenceValue)
                    {
                        serializedObject.ApplyModifiedProperties();
                    }

                    // Remove button
                    if (GUILayout.Button("Remove"))
                    {
                        statesProp.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
                
                _editorCommon.DrawUILine();

                // Add button
                if (GUILayout.Button("Add New State"))
                {
                    statesProp.arraySize++;
                    serializedObject.ApplyModifiedProperties();
                }
            }
            
            serializedObject.ApplyModifiedProperties();

        }
    }
}