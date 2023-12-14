using UnityEditor;
using UnityEngine;

namespace CycladeBase.Utils
{
    public class CycladeHelpBoxAttribute : PropertyAttribute
    {
        public string text;
        public MessageType messageType;

        public CycladeHelpBoxAttribute(string text, MessageType messageType = MessageType.Info)
        {
            this.text = text;
            this.messageType = messageType;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(CycladeHelpBoxAttribute))]
    public class CycladeHelpBoxPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var helpBoxAttribute = (CycladeHelpBoxAttribute)attribute;

            EditorGUI.HelpBox(position, helpBoxAttribute.text, helpBoxAttribute.messageType);

            if (!property.name.Contains("stub")) 
                Debug.LogError($"property.name not contains \"helptext\"");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
#endif
}