using System;
using UnityEngine;
using Util.Variable;

namespace Util.Colour
{
    [ExecuteAlways]
    public abstract class ColourSwitcher : MonoBehaviour
    {
        [SerializeField] private ColourPaletteVariable colours;
        [SerializeField] private int colourIndex;

        [Range(0, 1)]
        [SerializeField] float alphaOverride = 1;

        protected abstract void SetColour(Color colour);

        private void OnValidate()
        {
            SetUpColour();
        }

        private void SetUpColour()
        {
            if (colours == null) return;
            var color = colours.Value.GetColour(colourIndex);
            color.a = alphaOverride;

            SetColour(color);

            colours.Value.OnChange -= SetUpColour;
            colours.Value.OnChange += SetUpColour;
        }


        public void ChangeColour(int newColour)
        {
            colourIndex = newColour;
            SetUpColour();
        }
    }
}