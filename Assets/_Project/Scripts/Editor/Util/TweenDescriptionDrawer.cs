using System;
using UnityEditor;
using UnityEngine;

namespace Util
{
    [CustomPropertyDrawer(typeof(TweenDescription))]
    public class TweenDescriptionDrawer : PropertyDrawer
    {
        private const int PADDING = 2;

        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            var foldoutPos = position;
            foldoutPos.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(foldoutPos, property.isExpanded, label);

            if (!property.isExpanded) return;

            var propName = property.FindPropertyRelative("Name");
            var propObj = property.FindPropertyRelative("ObjectToAnimate");
            var propEase = property.FindPropertyRelative("Ease");
            var propUseCustomCurve = property.FindPropertyRelative("UseCustomCurve");
            var propCustomCurve = property.FindPropertyRelative("CustomCurve");
            var propProperty = property.FindPropertyRelative("Property");
            var propPropertyName = property.FindPropertyRelative("PropertyName");
            var propDuration = property.FindPropertyRelative("Duration");
            var propRelativeToCurrent = property.FindPropertyRelative("RelativeToCurrent");
            var propStart = property.FindPropertyRelative("Start");
            var propEnd = property.FindPropertyRelative("End");
            var propPlayType = property.FindPropertyRelative("PlayType");
            var propPlayOnEnable = property.FindPropertyRelative("PlayOnEnable");
            var propRandomDelay = property.FindPropertyRelative("RandomDelay");
            var propDefaultDelay = property.FindPropertyRelative("DefaultDelay");
            var propOnComplete = property.FindPropertyRelative("OnComplete");
            var propTimeScaleDependent = property.FindPropertyRelative("TimeScaleDependent");


            // set up rect for the first property
            var rect = position;
            rect.y += EditorGUIUtility.singleLineHeight;
            rect.height = EditorGUIUtility.singleLineHeight;

            using (new EditorGUI.IndentLevelScope(1))
            {
                rect = PropertyOnNextLine(rect, propName);
                rect = PropertyOnNextLine(rect, propObj);
                rect = PropertyOnNextLine(rect, propEase);

                // only display the gradient if actually using it
                rect = PropertyOnNextLine(rect, propUseCustomCurve);
                if (propUseCustomCurve.boolValue)
                {
                    rect = PropertyOnNextLine(rect, propCustomCurve);
                }

                rect = PropertyOnNextLine(rect, propProperty);
                if (propProperty.enumValueIndex == IndexOf(TweenBehaviour.Property.SpriteShaderFloat))
                {
                    rect = PropertyOnNextLine(rect, propPropertyName);
                }
                
                rect = PropertyOnNextLine(rect, propDuration);
                rect = PropertyOnNextLine(rect, propRelativeToCurrent);

                // display the start and end points based on what you are changing
                rect = StartAndEndOnNextLines(
                    rect,
                    (TweenBehaviour.Property) propProperty.enumValueIndex,
                    propStart,
                    propEnd
                );

                rect = PropertyOnNextLine(rect, propPlayType);
                rect = PropertyOnNextLine(rect, propPlayOnEnable);

                // only display 2 floats if we need a rangom range
                rect = PropertyOnNextLine(rect, propRandomDelay);
                if (propRandomDelay.boolValue)
                {
                    rect = MultiFloatFieldOnNextLine(
                        rect,
                        propDefaultDelay,
                        new[] {new GUIContent("Min"), new GUIContent("Max")}
                    );
                }
                else
                {
                    rect = FloatFieldOnNextLine2(rect, propDefaultDelay);
                }

                rect = PropertyOnNextLine(rect, propTimeScaleDependent);
                rect = PropertyOnNextLine(rect, propOnComplete);
            }
        }

        int IndexOf(TweenBehaviour.Property p)
        {
            int i = 0;
            
            foreach (TweenBehaviour.Property e in typeof(TweenBehaviour.Property).GetEnumValues())
            {
                if (e == p) return i;
                i++;
            }

            return -1;
        }


        private Rect PropertyOnNextLine(Rect rect, SerializedProperty prop)
        {
            EditorGUI.PropertyField(rect, prop);

            return AdjustForPropHeight(rect, prop);
        }

        private static Rect AdjustForPropHeight(Rect rect, SerializedProperty prop)
        {
            var height = EditorGUI.GetPropertyHeight(prop, true);
            rect.y += height + PADDING;
            rect.height = height + PADDING;
            return rect;
        }

        private Rect FloatFieldOnNextLine3(Rect rect, SerializedProperty prop)
        {
            prop.vector3Value = new Vector3(
                EditorGUI.FloatField(rect, prop.name, prop.vector3Value.x),
                prop.vector3Value.y,
                prop.vector3Value.z
            );


            return AdjustForPropHeight(rect, prop);
        }

        private Rect FloatFieldOnNextLine2(Rect rect, SerializedProperty prop)
        {
            prop.vector2Value = new Vector2(
                EditorGUI.FloatField(rect, prop.name, prop.vector2Value.x),
                prop.vector2Value.y
            );

            return AdjustForPropHeight(rect, prop);
        }

        private Rect Vector2OnNextLine(Rect rect, SerializedProperty prop)
        {
            Vector3 v3 = EditorGUI.Vector2Field(rect, prop.name, prop.vector3Value);
            v3.z = prop.vector3Value.z;
            prop.vector3Value = v3;

            return AdjustForPropHeight(rect, prop);
        }

        private Rect StartAndEndOnNextLines(
            Rect rect,
            TweenBehaviour.Property tweenProp,
            SerializedProperty propStart,
            SerializedProperty propEnd)
        {
            switch (tweenProp)
            {
                case TweenBehaviour.Property.Alpha:
                    rect = FloatFieldOnNextLine3(rect, propStart);
                    rect = FloatFieldOnNextLine3(rect, propEnd);
                    break;
                case TweenBehaviour.Property.Scale:
                    rect = PropertyOnNextLine(rect, propStart);
                    rect = PropertyOnNextLine(rect, propEnd);
                    break;
                case TweenBehaviour.Property.Position:
                    rect = PropertyOnNextLine(rect, propStart);
                    rect = PropertyOnNextLine(rect, propEnd);
                    break;
                case TweenBehaviour.Property.RectPosition:
                    rect = Vector2OnNextLine(rect, propStart);
                    rect = Vector2OnNextLine(rect, propEnd);
                    break;
                case TweenBehaviour.Property.Rotation:
                    rect = PropertyOnNextLine(rect, propStart);
                    rect = PropertyOnNextLine(rect, propEnd);
                    break;
                case TweenBehaviour.Property.SpriteShaderFloat:
                    rect = FloatFieldOnNextLine3(rect, propStart);
                    rect = FloatFieldOnNextLine3(rect, propEnd);
                    break;
                case TweenBehaviour.Property.SpriteAlpha:
                    rect = FloatFieldOnNextLine3(rect, propStart);
                    rect = FloatFieldOnNextLine3(rect, propEnd);
                    break;
                default:
                    rect = PropertyOnNextLine(rect, propStart);
                    rect = PropertyOnNextLine(rect, propEnd);
                    break;
            }

            return rect;
        }

        private Rect MultiFloatFieldOnNextLine(Rect rect, SerializedProperty prop, GUIContent[] labels)
        {
            float[] values = {prop.vector2Value.x, prop.vector2Value.y};

            EditorGUI.MultiFloatField(rect, new GUIContent(prop.name), labels, values);
            prop.vector2Value = new Vector2(values[0], values[1]);

            return AdjustForPropHeight(rect, prop);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = (EditorGUIUtility.singleLineHeight + PADDING);

            if (property.isExpanded)
            {
                var propUseCustomCurve = property.FindPropertyRelative("UseCustomCurve");
                if (propUseCustomCurve.boolValue)
                {
                    height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("CustomCurve"), true);
                }

                height += EditorGUI.GetPropertyHeight(propUseCustomCurve, true);


                height += TotalHeight(property, new[]
                {
                    "Name",
                    "ObjectToAnimate",
                    "Ease",
                    "Property",
                    "PropertyName",
                    "Duration",
                    "RelativeToCurrent",
                    "Start",
                    "End",
                    "PlayType",
                    "PlayOnEnable",
                    "RandomDelay",
                    "DefaultDelay",
                    "TimeScaleDependent",
                    "OnComplete"
                });
            }

            return height;
        }

        private float TotalHeight(SerializedProperty parent, string[] propertyNames)
        {
            float total = 0;
            foreach (var name in propertyNames)
            {
                total += EditorGUI.GetPropertyHeight(parent.FindPropertyRelative(name), true);
                total += PADDING;
            }

            return total;
        }
    }
}