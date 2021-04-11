using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// flexible grid layout component based on https://www.youtube.com/watch?v=CGsEJToeXmA
namespace Util.UI
{
    public class FlexibleGridLayout : LayoutGroup
    {
        public enum FitType
        {
            Uniform,
            FitHorizontal,
            FitVertical,
            FixedColumns,
            FixedRows
        }

        [Serializable]
        public struct ArrayWeight
        {
            public int Index;
            [Range(0, 1)] public float Weight;
        }

        [SerializeField] private Vector2Int spacing;
        [SerializeField] private FitType fitType;
        [SerializeField] private int rows;
        [SerializeField] private int columns;
        [SerializeField] private Vector2 cellSize;
        [SerializeField] private bool fitX = false;
        [SerializeField] private bool fitY = false;
        [SerializeField] private ArrayWeight[] columnWeights = new ArrayWeight[0];
        [SerializeField] private ArrayWeight[] rowWeights = new ArrayWeight[0];


        private float[] _columnProportions;
        private float[] _rowProportions;

        protected override void OnEnable()
        {
            base.OnEnable();
            CalculateLayoutInputHorizontal();
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            ConfigureColumnsAndRowCounts();

            var parentRect = AddPadding(rectTransform.rect);

            ConfigureColumnSizes(parentRect);
            ConfigureRowSizes(parentRect);

            LayoutChildren();
        }

        private void ConfigureColumnsAndRowCounts()
        {
            int sqrt;


            switch (fitType)
            {
                case FitType.Uniform:
                    fitX = true;
                    fitY = true;

                    sqrt = Mathf.CeilToInt(Mathf.Sqrt(transform.childCount));
                    columns = sqrt;
                    rows = sqrt;

                    break;
                case FitType.FitHorizontal:
                    fitX = true;
                    fitY = true;

                    sqrt = Mathf.CeilToInt(Mathf.Sqrt(transform.childCount));
                    columns = sqrt;
                    rows = Mathf.CeilToInt((float) transform.childCount / columns);

                    break;
                case FitType.FitVertical:
                    fitX = true;
                    fitY = true;

                    sqrt = Mathf.CeilToInt(Mathf.Sqrt(transform.childCount));
                    rows = sqrt;
                    columns = Mathf.CeilToInt((float) transform.childCount / rows);
                    break;
                case FitType.FixedColumns:
                    rows = Mathf.CeilToInt((float) transform.childCount / columns);
                    break;
                case FitType.FixedRows:
                    columns = Mathf.CeilToInt((float) transform.childCount / rows);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Rect AddPadding(Rect parentRect)
        {
            parentRect.width -= padding.left + padding.right;
            parentRect.height -= padding.top + padding.bottom;

            return parentRect;
        }

        private void ConfigureColumnSizes(Rect parentRect)
        {
            Array.Resize(ref _columnProportions, columns);
            if (fitX)
            {
                ConfigureSizes(ref _columnProportions, columnWeights, parentSize: parentRect.width, space: spacing.x);
            }
            else
            {
                _columnProportions.Fill(cellSize.x);
            }
        }

        private void ConfigureSizes(ref float[] sizes, ArrayWeight[] weights, float parentSize, int space)
        {
            int count = sizes.Length;
            var totalSize = parentSize - (count - 1) * space;
            sizes.Fill(-1f);

            var totalWeight = 1f;
            var remainingCount = count;
            for (int i = 0; i < weights.Length; i++)
            {
                var weight = weights[i];
                if (weight.Index >= sizes.Length) continue;

                if (Mathf.Approximately(sizes[weight.Index], -1f))
                {
                    sizes[weight.Index] = weight.Weight * totalSize;
                    totalWeight -= weight.Weight;
                    remainingCount--;
                }
            }

            var remainingWidth = (totalWeight / remainingCount) * totalSize;
            for (int i = 0; i < sizes.Length; i++)
            {
                if (Mathf.Approximately(sizes[i], -1f))
                {
                    sizes[i] = remainingWidth;
                }
            }
        }

        private void ConfigureRowSizes(Rect parentRect)
        {
            Array.Resize(ref _rowProportions, rows);
            if (fitY)
            {
                ConfigureSizes(ref _rowProportions, rowWeights, parentSize: parentRect.height, space: spacing.y);
            }
            else
            {
                _rowProportions.Fill(cellSize.y);
            }
        }

        private void LayoutChildren()
        {
            RectTransform child;
            int columnIndex;
            int rowIndex;
            float cellX = padding.left;
            float cellY = padding.top;
            float cellWidth;
            float cellHeight;
            int maxIdx = rectChildren.Count - 1;

            int idx = 0;
            for (rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                cellX = padding.left;
                cellHeight = _rowProportions[rowIndex];

                for (columnIndex = 0; columnIndex < columns; columnIndex++)
                {
                    idx = rowIndex * columns + columnIndex;
                    if (idx > maxIdx) break;

                    cellWidth = _columnProportions[columnIndex];

                    child = rectChildren[idx];

                    SetChildAlongAxis(child, 0, cellX, cellWidth);
                    SetChildAlongAxis(child, 1, cellY, cellHeight);


                    cellX += cellWidth + spacing.x;
                }

                cellY += cellHeight + spacing.y;
            }
        }


        public override void CalculateLayoutInputVertical()
        {
            // Debug.Log("FlexibleGridLayout.CalculateLayoutInputVertical");
        }

        public override void SetLayoutHorizontal()
        {
            // Debug.Log("FlexibleGridLayout.SetLayoutHorizontal");
        }

        public override void SetLayoutVertical()
        {
            // Debug.Log("FlexibleGridLayout.SetLayoutVertical");
        }
    }
}