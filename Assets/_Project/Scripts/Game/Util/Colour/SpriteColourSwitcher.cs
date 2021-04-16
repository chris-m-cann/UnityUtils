using System;
using UnityEngine;
using Util.Variable;

namespace Util.Colour
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteColourSwitcher : ColourSwitcher
    {
        private SpriteRenderer _sprite;

        protected override void SetColour(Color colour)
        {
            _sprite = this.GetComponentIfNull(_sprite);

            _sprite.color = colour;
        }
    }
}