using System;
using UnityEditor;
using UnityEngine;
using Util.UI;

namespace Util.UI
{
    [CustomEditor(typeof(FlexibleGridLayout))]
    public class FlexibleGridLayoutEditor : Editor
    {
        private SerializedProperty _propPadding;
        private SerializedProperty _propSpacing;
        private SerializedProperty _propFitType;
        private SerializedProperty _propRows;
        private SerializedProperty _propColumns;
        private SerializedProperty _propCellSize;
        private SerializedProperty _propFitX;
        private SerializedProperty _propFitY;
        private SerializedProperty _propColumnWeights;
        private SerializedProperty _propRowWeights;

        private void OnEnable()
        {
            _propPadding = serializedObject.FindProperty("m_Padding");
            _propSpacing = serializedObject.FindProperty("spacing");
            _propFitType = serializedObject.FindProperty("fitType");
            _propRows = serializedObject.FindProperty("rows");
            _propColumns = serializedObject.FindProperty("columns");
            _propCellSize = serializedObject.FindProperty("cellSize");
            _propFitX = serializedObject.FindProperty("fitX");
            _propFitY = serializedObject.FindProperty("fitY");
            _propColumnWeights = serializedObject.FindProperty("columnWeights");
            _propRowWeights = serializedObject.FindProperty("rowWeights");
        }

        public override void OnInspectorGUI()
        {
            using (serializedObject.UpdateScope())
            {

                EditorGUILayout.PropertyField(_propPadding, true);
                EditorGUILayout.PropertyField(_propSpacing, true);
                EditorGUILayout.PropertyField(_propFitType, true);

                using (new EditorGUI.IndentLevelScope(1))
                {
                    var fitType = (FlexibleGridLayout.FitType)_propFitType.enumValueIndex;

                    switch (fitType)
                    {
                        case FlexibleGridLayout.FitType.FixedColumns:
                            EditorGUILayout.PropertyField(_propColumns, true);
                            LayoutFixedFitTypeSection();
                            break;
                        case FlexibleGridLayout.FitType.FixedRows:
                            EditorGUILayout.PropertyField(_propRows, true);
                            LayoutFixedFitTypeSection();
                            break;
                        default: // all of the Uniform/Fit* sections dont need the fit of fixed size details
                            break;
                    }

                }

                if (_propFitX.boolValue)
                {
                    EditorGUILayout.PropertyField(_propColumnWeights, true);
                }

                if (_propFitY.boolValue)
                {
                    EditorGUILayout.PropertyField(_propRowWeights, true);
                }
            }
        }

        private void LayoutFixedFitTypeSection()
        {
            EditorGUILayout.PropertyField(_propFitX, true);
            EditorGUILayout.PropertyField(_propFitY, true);

            if (!_propFitX.boolValue && !_propFitY.boolValue)
            {
                EditorGUILayout.PropertyField(_propCellSize, true);
            }
            else if (!_propFitX.boolValue)
            {
                _propCellSize.vector2Value = new Vector2(
                    x: EditorGUILayout.FloatField("Cell Size X", _propCellSize.vector2Value.x),
                    y: _propCellSize.vector2Value.y
                );
            }
            else if (!_propFitY.boolValue)
            {
                _propCellSize.vector2Value = new Vector2(
                    x: _propCellSize.vector2Value.x,
                    y: EditorGUILayout.FloatField("Cell Size Y", _propCellSize.vector2Value.y)
                );
            }
        }
    }
}