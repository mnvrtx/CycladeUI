using CycladeBindings.UIStateSystem.Elements;
using CycladeBindings.UIStateSystem.Models;
using CycladeBindingsEditor.UIStateSystem.Base;
using UnityEditor;
using UnityEngine;

namespace CycladeBindingsEditor.UIStateSystem
{
    [CustomEditor(typeof(SelectableImage))]
    public class SelectableImageEditor : BaseSelectableEditor<ImageState>
    {
        protected override void DrawPropValues(SerializedProperty stateProp)
        {
            var imageProp = stateProp.FindPropertyRelative("image");
            var colorProp = stateProp.FindPropertyRelative("color");

            var before = imageProp.objectReferenceValue;
            imageProp.objectReferenceValue = EditorGUILayout.ObjectField(before, typeof(Sprite), allowSceneObjects: false);
            if (before != imageProp.objectReferenceValue) 
                serializedObject.ApplyModifiedProperties();

            var beforeColor = colorProp.colorValue;
            colorProp.colorValue = EditorGUILayout.ColorField(beforeColor, GUILayout.Width(40));
            if (beforeColor != colorProp.colorValue) 
                serializedObject.ApplyModifiedProperties();
        }
    }
}