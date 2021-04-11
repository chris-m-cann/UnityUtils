using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Util
{
    // todo(chris): Theres a lot more i can do here
    // - add properties
    // - modularise the builders so the supported tweeners are handled in a map
    // - reuse tweeners to save garbage for oftenly used tweens
    // - fix the flexible grid layout hack
    public class TweenBehaviour : MonoBehaviour
    {
        public enum Property
        {
            Alpha,
            Scale,
            Position,
            RectPosition,
            Rotation
        }

        public enum PlayType
        {
            Once,
            PingPong,
            Loop,
            ThereAndBack
        }

        [System.Serializable]
        public struct TweenDescription
        {
            [Tooltip("If null, object this component is attached to will be used")]
            public GameObject ObjectToAnimate;
            public Ease Ease;
            public bool UseCustomCurve;
            public AnimationCurve CustomCurve;
            public Property Property;
            public float Duration;
            public bool RelativeToCurrent;
            public Vector3 Start;
            public Vector3 End;
            public PlayType PlayType;
            public bool PlayOnEnable;
            public bool RandomDelay;
            public Vector2 DefaultDelay;
            public UnityEvent OnComplete;
        }

        [SerializeField] private TweenDescription[] tweens;


        private int _runningTweenId = 42;
        private Tweener[] _tweeners = new Tweener[1];

        private void OnValidate()
        {
            if (tweens.Length == _tweeners.Length) return;

            Array.Resize(ref _tweeners, tweens.Length);
        }

        private void OnEnable()
        {
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
                tweener?.Kill();
            }
        }

        public void Play(int idx)
        {
            Assert.IsTrue(idx > -1 && idx < tweens.Length, $"idx = {idx} not in range");

            StartCoroutine(CoPlay(idx));
        }



        private IEnumerator CoPlayFromOnEnable(int idx)
        {
            TweenDescription tween = tweens[idx];

            // if we arent going to delay anyway then need to wait until any parent layouts are done
            if (!tween.RandomDelay && Mathf.Approximately(tween.DefaultDelay.x, 0))
            {
                yield return new WaitForEndOfFrame();
            }

            yield return StartCoroutine(CoPlay(idx));
        }

        private IEnumerator CoPlay(int idx)
        {
            TweenDescription tween = tweens[idx];

            float delay = tween.DefaultDelay.x;
            if (tween.RandomDelay)
            {
                delay = Random.Range(tween.DefaultDelay.x, tween.DefaultDelay.y);
            }


            if (_tweeners[idx] != null)
            {
                _tweeners[idx].Kill();
            }

            var tweener = BuildTweener(tween).SetDelay(delay).OnComplete(() => OnComplete(idx));
            _tweeners[idx] = tweener;
            if (tweener == null) yield break;

            tweener.SetId(_runningTweenId);
            tweener.SetAutoKill(false);

            switch (tween.PlayType)
            {
                case PlayType.Once:
                    break;
                case PlayType.PingPong:
                    tweener.SetLoops(-1, LoopType.Yoyo);
                    break;
                case PlayType.Loop:
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
        }

        private Tweener BuildTweener(TweenDescription tween)
        {
            if (tween.ObjectToAnimate == null)
            {
                tween.ObjectToAnimate = gameObject;
            }

            switch (tween.Property)
            {
                case Property.Alpha:
                {
                    return BuildAlphaTweener(tween);
                }
                case Property.Scale: return BuildScaleTweener(tween);
                case Property.Position: return BuildPositionTweener(tween);
                case Property.RectPosition: return BuildRectPositionTweener(tween);
                case Property.Rotation: return BuildRotationTweener(tween);
                default:
                {
                    Debug.LogError($"Attempting to tween invalid property = {tween.Property}");
                    return null;
                }
            }
        }

        private Tweener BuildAlphaTweener(TweenDescription tween)
        {
            var canvas = tween.ObjectToAnimate.GetComponent<CanvasGroup>();
            var tweenStart = tween.Start.x;
            var tweenEnd = tween.End.x;
            if (tween.RelativeToCurrent)
            {
                tweenStart += canvas.alpha;
                tweenEnd += canvas.alpha;
            }

            canvas.alpha = tweenStart;

            return canvas.DOFade(tweenEnd, tween.Duration).From(tweenStart);
        }

        private Tweener BuildScaleTweener(TweenDescription tween)
        {
            var tweenStart = tween.Start;
            var tweenEnd = tween.End;
            if (tween.RelativeToCurrent)
            {
                tweenStart += tween.ObjectToAnimate.transform.localScale;
                tweenEnd += tween.ObjectToAnimate.transform.localScale;
            }

            tween.ObjectToAnimate.transform.localScale = tweenStart;
            return tween.ObjectToAnimate.transform.DOScale(tweenEnd, tween.Duration).From(tweenStart);
        }

        private Tweener BuildPositionTweener(TweenDescription tween)
        {
            var tweenStart = tween.Start;
            var tweenEnd = tween.End;
            if (tween.RelativeToCurrent)
            {
                tweenStart += tween.ObjectToAnimate.transform.position;
                tweenEnd += tween.ObjectToAnimate.transform.position;
            }

            // in the middle of chaning te start points
            tween.ObjectToAnimate.transform.position = tweenStart;
            return tween.ObjectToAnimate.transform.DOMove(tweenEnd, tween.Duration).From(tweenStart);
        }

        private Tweener BuildRectPositionTweener(TweenDescription tween)
        {
            var rect = tween.ObjectToAnimate.GetComponent<RectTransform>();

            DOGetter<Vector2> getter;
            Vector2 tweenStart;
            Vector2 tweenEnd;
            if (tween.RelativeToCurrent)
            {
                var initial = rect.anchoredPosition;
                getter = () => rect.anchoredPosition + initial;
                tweenStart = (Vector2) tween.Start + initial;
                tweenEnd = (Vector2) tween.End + initial;
            }
            else
            {
                getter = () => rect.anchoredPosition;
                tweenStart = tween.Start;
                tweenEnd = tween.End;
            }

            rect.anchoredPosition = tweenStart;

            return DOTween.To(getter, it => rect.anchoredPosition = it, tweenEnd, tween.Duration).From(tweenStart);
        }

        private Tweener BuildRotationTweener(TweenDescription tween)
        {
            var tweenStart = tween.Start;
            var tweenEnd = tween.End;
            if (tween.RelativeToCurrent)
            {
                tweenStart += tween.ObjectToAnimate.transform.position;
                tweenEnd += tween.ObjectToAnimate.transform.position;
            }

            tween.ObjectToAnimate.transform.eulerAngles = tweenStart;
            return tween.ObjectToAnimate.transform.DORotate(tweenEnd, tween.Duration).From(tweenStart);
        }


        private void OnComplete(int idx)
        {
            tweens[idx].OnComplete.Invoke();
        }
    }
}