using System;
using UnityEngine;
using Util;

namespace Game
{
    [RequireComponent(typeof(SoundPlayer))]
    public class SoundPlayerTest : MonoBehaviour
    {
        [SerializeField] private SoundPlayer.AudioClipEx clip;


        private SoundPlayer _player;

        private void Awake()
        {
            _player = GetComponent<SoundPlayer>();
        }

        public void FadeIn(float duration) => _player.FadeInSound(clip, duration);
        public void FadeIn0(float duration) => _player.FadeInSound(0, duration);

        public void FadeOut(float duration) => _player.FadeOutSound(duration);

        public void CrossFade(float duration) => _player.CorssfadeToSound(clip, duration, duration);
    }
}