using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Xeiv.TextTaggerSystem
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = true)]
    public class CustomHelpAttribute : PropertyAttribute
    {
        public readonly string text;
        public readonly MessageType type;
        public readonly Color backgroundColor;

        public CustomHelpAttribute(string text, MessageType type = MessageType.Info, float r = 1f, float g = 1f, float b = 1f)
        {
            this.text = text;
            this.type = type;
            this.backgroundColor = new Color(r, g, b);
        }
        public CustomHelpAttribute(string text, MessageType type = MessageType.Info)
        {
            this.text = text;
            this.type = type;
            this.backgroundColor = GUI.backgroundColor;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(CustomHelpAttribute))]
    public class CustomHelpDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CustomHelpAttribute helpAttribute = (CustomHelpAttribute)attribute;

            // Draw help box with custom text, type, and background color
            var originalColor = GUI.backgroundColor;
            GUI.backgroundColor = helpAttribute.backgroundColor;
            EditorGUI.HelpBox(position, helpAttribute.text, helpAttribute.type);
            GUI.backgroundColor = originalColor;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            CustomHelpAttribute helpAttribute = (CustomHelpAttribute)attribute;
            var content = new GUIContent(helpAttribute.text);
            var style = GUI.skin.GetStyle("helpbox");
            float height = style.CalcHeight(content, EditorGUIUtility.currentViewWidth);
            return height + EditorGUIUtility.standardVerticalSpacing;
        }
    }
#endif
}