using System;
using UnityEngine;
using UnityEngine.UI;
using Util.Variable;

namespace Util.Colour
{
    [RequireComponent(typeof(Image))]
    public class ImageColourSwitcher : ColourSwitcher
    {
        private Image _image;

        protected override void SetColour(Color colour)
        {
            _image = this.GetComponentIfNull(_image);

            _image.color = colour;
        }
    }
}