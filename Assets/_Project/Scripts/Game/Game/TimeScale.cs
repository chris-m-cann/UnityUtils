using System;
using UnityEngine;

namespace Game
{
    public class TimeScale : MonoBehaviour
    {
        [Range(0, 1)] public float scale = 1f;

        private float oldScale;
        
        private void OnEnable()
        {
            oldScale = Time.timeScale;
            Time.timeScale = scale;
        }

        private void OnDisable()
        {
            Time.timeScale = oldScale;
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                Time.timeScale = scale;
            }
        }
    }
}