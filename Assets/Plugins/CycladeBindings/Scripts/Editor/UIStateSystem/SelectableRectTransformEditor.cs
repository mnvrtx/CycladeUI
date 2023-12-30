using CycladeBindings.UIStateSystem.Elements;
using CycladeBindings.UIStateSystem.Models;
using CycladeBindingsEditor.UIStateSystem.Base;
using UnityEditor;
using UnityEngine;

namespace CycladeBindingsEditor.UIStateSystem
{
    [CustomEditor(typeof(SelectableRectTransform))]
    public class SelectableRectTransformEditor : BaseSelectableEditor<RectTransformState>
    {
        protected override void DrawPropValues(SerializedProperty stateProp)
        {
            var rectTransformProp = stateProp.FindPropertyRelative("rectTransform");

            var before = rectTransformProp.objectReferenceValue;
            rectTransformProp.objectReferenceValue = EditorGUILayout.ObjectField(before, typeof(RectTransform), allowSceneObjects: true);
            if (before != rectTransformProp.objectReferenceValue) 
                serializedObject.ApplyModifiedProperties();
        }
    }
}