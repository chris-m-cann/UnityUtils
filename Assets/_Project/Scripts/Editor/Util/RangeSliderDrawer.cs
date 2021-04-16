using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Util
{
    [CustomPropertyDrawer(typeof(RangeSliderAttribute))]
    public class RangeSliderDrawer : PropertyDrawer
    {
        // this needs to use GUI/EditorGUI calls rather than GUILayout/EditorGUILayout
        // as the *Layout calls cause and exception in the editor
        // apparently they are not supported for performance reasons
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var minProperty = property.FindPropertyRelative("Start");
            var maxProperty = property.FindPropertyRelative("End");


            var labelRect = new Rect(
                position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight
            );

            EditorGUI.LabelField(labelRect, label);

            var sliderRect = LayoutSlider(position, labelRect, minProperty, maxProperty);

            var secondRowRect = sliderRect;
            secondRowRect.y += EditorGUIUtility.singleLineHeight;
            LayoutStartAndEnd(secondRowRect, minProperty, maxProperty);


            EditorGUI.EndProperty();
        }

        private Rect LayoutSlider(Rect position, Rect labelRect, SerializedProperty minProperty,
            SerializedProperty maxProperty)
        {
            var att = attribute as RangeSliderAttribute;

            var sliderRect = new Rect(
                labelRect.x + labelRect.width,
                position.y,
                position.width - labelRect.width,
                EditorGUIUtility.singleLineHeight
            );

            float min = minProperty.floatValue;
            float max = maxProperty.floatValue;

            EditorGUI.MinMaxSlider(sliderRect, ref min, ref max, att.Min, att.Max);
            minProperty.floatValue = min;
            maxProperty.floatValue = max;
            return sliderRect;
        }

        private void LayoutStartAndEnd(Rect overallRect, SerializedProperty minProperty,
            SerializedProperty maxProperty)
        {
            var values = new float[] {minProperty.floatValue, maxProperty.floatValue};

            EditorGUI.MultiFloatField(overallRect, new[] {new GUIContent("start"), new GUIContent("end")}, values);

            minProperty.floatValue = values[0];
            maxProperty.floatValue = values[1];
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}