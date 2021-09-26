using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using DG.Tweening.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
namespace Util.Tween
{
    public class TweenBehaviour : SerializedMonoBehaviour
    {
        public enum PlayType
        {
            Once,
            PingPong,
            Loop,
            ThereAndBack
        }

        
        [TypeFilter("GetFilteredTypeList")] 
        [SerializeField] private Tween[] tweens = new Tween[0];


        public IEnumerable<Type> GetFilteredTypeList()
        {
            var q = typeof(Tween).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)                                          // Excludes BaseClass
                .Where(x => !x.IsGenericTypeDefinition)                             // Excludes C1<>
                .Where(x => typeof(Tween).IsAssignableFrom(x));                 // Excludes classes not inheriting from BaseClass
    
            return q;
        }
        

        private Dictionary<int, Tweener> _tweeners = new Dictionary<int, Tweener>();
        
        private void OnEnable()
        {
            if (tweens == null) return;
            for (int i = 0; i < tweens.Length; i++)
            {
                if (tweens[i].PlayOnEnable)
                {
                    StartCoroutine(CoPlayFromOnEnable(i));
                }
            }
        }

        private void OnDisable()
        {
            foreach (var tweener in _tweeners)
            {
                tweener.Value?.Kill();
            }
        }

        public void Play(int idx)
        {
            Assert.IsTrue(idx > -1 && idx < tweens.Length, $"idx = {idx} not in range");

            if (isActiveAndEnabled && gameObject.activeInHierarchy)
            {
                StartCoroutine(CoPlay(tweens[idx]));
            }
        }
        
        public IEnumerator CoPlay(Tween tween)
        {
            bool running = true;

            float delay = tween.Delay;
            if (tween.RandomDelay)
            {
                delay = Random.Range(tween.DelayRange.x, tween.DelayRange.y);
            }

            var tweenHash = tween.GetHashCode();

            if (_tweeners.ContainsKey(tweenHash))
            {
                _tweeners[tweenHash].Kill();
            }

            var tweener = tween.BuildTweener(gameObject);
            if (tweener == null) yield break;

            tweener.SetDelay(delay)
                .SetUpdate(UpdateType.Normal, !tween.TimeScaleDependent)
                .OnComplete(() =>
                {
                    tween.OnComplete?.Invoke();
                    _tweeners.Remove(tweenHash);
                    running = false;
                });

            _tweeners[tweenHash] = tweener;

            tweener.SetAutoKill(true);

            switch (tween.PlayType)
            {
                case PlayType.Once:
                    break;
                case PlayType.PingPong:
                    running = false;
                    tweener.SetLoops(-1, LoopType.Yoyo);
                    break;
                case PlayType.Loop:
                    running = false;
                    tweener.SetLoops(-1);
                    break;
                case PlayType.ThereAndBack:
                    tweener.SetLoops(2, LoopType.Yoyo);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            if (tween.UseCustomCurve)
            {
                tweener.SetEase(tween.CustomCurve);
            }
            else
            {
                tweener.SetEase(tween.Ease);
            }

            DOTween.Play(tweener);

            // just block until we are done
            while (running)
            {
                yield return null;
            }
        }

        public void Stop(int idx)
        {
            Assert.IsTrue(idx > -1 && idx < tweens.Length, $"idx = {idx} not in range");

            var hash = tweens[idx].GetHashCode();
            if (_tweeners.ContainsKey(hash))
            {
                _tweeners[hash]?.Kill();
                _tweeners.Remove(hash);
            }
        }
        
        public Tween GetTween(int index)
        {
            return tweens[index];
        }
        
        
        public void SetTween(int index, Tween tween)
        {
            tweens[index] = tween;
        }

        [ContextMenu("PlayAll")]
        public void PlayAll()
        {
            foreach (var tween in tweens)
            {
                StartCoroutine(CoPlay(tween));
            }
        }

        private IEnumerator CoPlayFromOnEnable(int idx)
        {
            var tween = tweens[idx];

            // if we arent going to delay anyway then need to wait until any parent layouts are done
            if (!tween.RandomDelay && Mathf.Approximately(tween.Delay, 0))
            {
                yield return new WaitForEndOfFrame();
            }

            yield return StartCoroutine(CoPlay(tweens[idx]));
        }
    }
}