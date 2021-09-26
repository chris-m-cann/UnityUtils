using System;
using TMPro;
using UnityEngine;

namespace Util.UI
{
    public class StringSetter : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private string format = "{0}";

        private void Awake()
        {
            if (text == null)
            {
                text = GetComponent<TMP_Text>();
            }
        }

        public void SetText(string value) => _SetText(value);
        public void SetText(int value) => _SetText(value);
        
        private void _SetText(object value)
        {
            text.text = string.Format(format, value);
        }
    }
}