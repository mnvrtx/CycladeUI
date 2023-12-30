using CycladeBindings.UIStateSystem.Elements;
using CycladeBindings.UIStateSystem.Models;
using CycladeBindingsEditor.UIStateSystem.Base;
using UnityEditor;
using UnityEngine;

namespace CycladeBindingsEditor.UIStateSystem
{
    [CustomEditor(typeof(SelectableUnityEvent))]
    public class SelectableUnityEventEditor  : BaseSelectableEditor<UnityEventState>
    {
        protected override void DrawPropValues(SerializedProperty stateProp)
        {
            var prop = stateProp.FindPropertyRelative("unityEvent");
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.PropertyField(prop, new GUIContent("UnityEvent"));
            
            EditorGUILayout.BeginHorizontal();
        }

        public override void DrawExtra()
        {
            EditorGUILayout.HelpBox($"Please set the EditorAndRuntime option for events for testing purposes.", MessageType.Info);
        }
    }
}