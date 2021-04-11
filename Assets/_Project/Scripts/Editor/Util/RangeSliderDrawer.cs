using UnityEditor;
using UnityEngine;

namespace Util
{
    [CustomPropertyDrawer(typeof(RangeSliderAttribute))]
    public class RangeSliderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            var att = attribute as RangeSliderAttribute;
            var minProperty = property.FindPropertyRelative("Start");
            var maxProperty = property.FindPropertyRelative("End");


            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(label, EditorStyles.boldLabel, GUILayout.Width(EditorGUIUtility.labelWidth));
                float min = minProperty.floatValue, max = maxProperty.floatValue;
                EditorGUILayout.MinMaxSlider( ref min, ref max, att.Min, att.Max);
                minProperty.floatValue = min;
                maxProperty.floatValue = max;
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("", GUILayout.Width(EditorGUIUtility.labelWidth));

                GUILayout.Label( "start", GUILayout.Width( 35 ) );
                EditorGUILayout.PropertyField( minProperty, GUIContent.none, true, GUILayout.MinWidth( 30 ),GUILayout.MaxWidth( 75 ) );

                GUILayout.FlexibleSpace();

                GUILayout.Label( "end", GUILayout.Width( 30 ) );
                EditorGUILayout.PropertyField( maxProperty, GUIContent.none, true, GUILayout.MinWidth( 30 ), GUILayout.MaxWidth( 75 ) );

            }
        }
    }
}