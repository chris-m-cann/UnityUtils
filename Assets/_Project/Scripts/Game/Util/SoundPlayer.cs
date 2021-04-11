using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Util
{
    // Todo(chris) extensions to this component
    // - take an abstracted "channel" so we can either supply an audio source or a channel to a group one
    // - an "on collision" policy for when the same clip is requested while it is playing
    // - persistance for the case when this is managing music across scenes
    // - taking the various fades as components so you can keep them separate
    public class SoundPlayer : MonoBehaviour
    {
        [Serializable]
        public class AudioClipEx
        {
            public AudioClip clip;
            [Range(0, 1)] public float volume = 1f;
            public AudioMixerGroup mixer;
            public bool Loop;
            public bool VaryPitch;
            [RangeSlider(-3, 3)]
            public Range PitchFactor;
        }

        [SerializeField] private AudioClipEx[] sounds;
        [SerializeField] private AudioSource source;

        private void Awake()
        {
            source = source ?? GetComponent<AudioSource>();
        }

        public void StopSounds()
        {
            source.Stop();
        }

        public void PlayOneOf() => PopInSound(sounds[Random.Range(0, sounds.Length)]);

        public void PopInSound(AudioClipEx clip)
        {
            source.Stop();
            SetClipDetails(source, clip);
            source.Play();
        }

        public void PopInSound(int idx)
        {
            Assert.IsTrue(IsInRange(idx, 0, sounds.Length), $"idx={idx} is not in range");

            PopInSound(sounds[idx]);
        }

        public void Play(int idx) => PopInSound(idx);

        public void FadeInSound(AudioClipEx clip, float duration)
        {
            if (source.isPlaying && clip.clip == source.clip) return;

            SetClipDetails(source, clip);
            source.volume = 0f;
            source.Play();

            source.DOFade(clip.volume, duration).Play();
        }

        public void FadeInSound(int idx, float duration)
        {
            Assert.IsTrue(IsInRange(idx, 0, sounds.Length), $"idx={idx} is not in range");

            FadeInSound(sounds[idx], duration);
        }

        public void FadeOutSound(float duration)
        {
            source.DOFade(0, duration).OnComplete(() => source.Stop()).Play();
        }


        public void CorssfadeToSound(AudioClipEx clip, float durationIn, float durationOut)
        {
            if (source.isPlaying && clip.clip == source.clip) return;

            Sequence seq = DOTween.Sequence().Append(source.DOFade(0, durationOut).OnComplete(() =>
            {
                source.Stop();
                SetClipDetails(source, clip);
                source.volume = 0;
                source.Play();
            })).Append(source.DOFade(clip.volume, durationIn));

            seq.Play();
        }

        public void CorssfadeToSound(int idx, float durationIn, float durationOut)
        {
            Assert.IsTrue(IsInRange(idx, 0, sounds.Length), $"idx={idx} is not in range");

            CorssfadeToSound(sounds[idx], durationIn, durationOut);
        }

        private bool IsInRange(int idx, int min, int max)
        {
            return !(idx < min) && (idx < max);
        }


        private void SetClipDetails(AudioSource source, AudioClipEx clip)
        {
            source.clip = clip.clip ?? source.clip;
            source.volume = clip.volume;
            source.outputAudioMixerGroup = clip.mixer ?? source.outputAudioMixerGroup;
            source.loop = clip.Loop;
            if (clip.VaryPitch)
            {
                var factor = RandomEx.Range(clip.PitchFactor);
                source.pitch = factor;
            }
            else
            {
                source.pitch = 1;
            }
        }
    }
}