using CycladeBindings.UIStateSystem.Elements;
using CycladeBindings.UIStateSystem.Models;
using CycladeBindingsEditor.UIStateSystem.Base;
using UnityEditor;
using UnityEngine;

namespace CycladeBindingsEditor.UIStateSystem
{
    [CustomEditor(typeof(SelectableTextColor))]
    public class SelectableTextColorEditor : BaseSelectableEditor<TextColorState>
    {
        protected override void DrawPropValues(SerializedProperty stateProp)
        {
            var colorProp = stateProp.FindPropertyRelative("color");

            var beforeColor = colorProp.colorValue;
            colorProp.colorValue = EditorGUILayout.ColorField(beforeColor, GUILayout.Width(40));
            if (beforeColor != colorProp.colorValue) 
                serializedObject.ApplyModifiedProperties();
        }
    }
}