using UnityEditor;
using UnityEngine;

namespace Util
{
    [CustomPropertyDrawer(typeof(Pair<,>))]
    public class PairDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.LabelField(position, label);

            var it = property.FindPropertyRelative("Item1");

            var fieldPos = position;
            fieldPos.x += EditorGUIUtility.labelWidth;
            fieldPos.width -= EditorGUIUtility.labelWidth;

            EditorGUI.MultiPropertyField(fieldPos, new []{new GUIContent("1st"), new GUIContent("2st")}, it);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}