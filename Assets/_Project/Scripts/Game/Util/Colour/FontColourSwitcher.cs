using TMPro;
using UnityEngine;
using Util.Var.Observe;

namespace Util.Colour
{
    [RequireComponent(typeof(TMP_Text))]
    public class FontColourSwitcher : ColourSwitcher
    {
        [SerializeField] private ObservableColourPaletteVariable observableColours;

        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }


        protected override void SetColour(Color colour)
        {
            _text = this.GetComponentIfNull(_text);

            _text.color = colour;
        }
    }
}