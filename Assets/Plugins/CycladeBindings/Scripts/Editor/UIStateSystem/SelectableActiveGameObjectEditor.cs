using CycladeBindings.UIStateSystem.Elements;
using CycladeBindings.UIStateSystem.Models;
using CycladeBindingsEditor.UIStateSystem.Base;
using UnityEditor;
using UnityEngine;

namespace CycladeBindingsEditor.UIStateSystem
{
    [CustomEditor(typeof(SelectableActiveGameObject))]
    public class SelectableActiveGameObjectEditor : BaseSelectableEditor<IsActiveState>
    {
        protected override void DrawPropValues(SerializedProperty stateProp)
        {
            var prop = stateProp.FindPropertyRelative("isActive");

            var before = prop.boolValue;
            prop.boolValue = EditorGUILayout.Toggle(before, GUILayout.Width(40));
            if (before != prop.boolValue) 
                serializedObject.ApplyModifiedProperties();
        }
    }
}