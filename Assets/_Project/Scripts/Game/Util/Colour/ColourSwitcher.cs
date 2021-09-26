using System;
using UnityEngine;
using Util.Var.Observe;

namespace Util.Colour
{
    [ExecuteAlways]
    public abstract class ColourSwitcher : MonoBehaviour
    {
        [SerializeField] private ObservableColourPaletteVariable observableColours;
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
            if (observableColours == null) return;
            var color = observableColours.Value.GetColour(colourIndex);
            color.a = alphaOverride;

            SetColour(color);

            observableColours.Value.OnChange -= SetUpColour;
            observableColours.Value.OnChange += SetUpColour;
        }


        public void ChangeColour(int newColour)
        {
            colourIndex = newColour;
            SetUpColour();
        }
    }
}