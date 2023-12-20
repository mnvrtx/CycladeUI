using CycladeBase.Utils;
using CycladeUI;
using UnityEditor;
using UnityEngine;

namespace CycladeUIEditor
{
    // [CustomEditor(typeof(UITestController))]
    // public class UITestControllerEditor : UnityEditor.Editor
    // {
    //     public override void OnInspectorGUI()
    //     {
    //         var settings = (UITestController)target;
    //         
    //         serializedObject.Update();
    //
    //         var property = serializedObject.GetIterator();
    //         bool enterChildren = true;
    //         while (property.NextVisible(enterChildren))
    //         {
    //             enterChildren = false;
    //
    //             EditorGUI.BeginDisabledGroup(property.name is "m_Script");
    //             EditorGUILayout.PropertyField(property, true);
    //             EditorGUI.EndDisabledGroup();
    //         }
    //         
    //         if (GUILayout.Button("Apply settings"))
    //         {
    //             settings.mockup.SetActive(settings.showMockup);
    //             settings.mockup.color = settings.mockupColor;
    //             settings.holder.FitInSafeArea(settings.debugSafeArea);
    //         }
    //         
    //         serializedObject.ApplyModifiedProperties();
    //     }
    // }
}