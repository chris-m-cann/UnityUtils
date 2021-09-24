using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
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
            Rotation,
            SpriteShaderFloat,
            SpriteAlpha
        }

        public enum PlayType
        {
            Once,
            PingPong,
            Loop,
            ThereAndBack
        }


        [SerializeField] private TweenDescription[] tweens;


        private int _runningTweenId = 42;
        private Dictionary<int, Tweener> _tweeners = new Dictionary<int, Tweener>();
        
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

        public IEnumerator CoPlay(TweenDescription tween)
        {
            bool running = true;
            if (tween.ObjectToAnimate != null && !tween.ObjectToAnimate.activeSelf) yield break;


            float delay = tween.DefaultDelay.x;
            if (tween.RandomDelay)
            {
                delay = Random.Range(tween.DefaultDelay.x, tween.DefaultDelay.y);
            }

            var tweenHash = tween.GetHashCode();

            if (_tweeners.ContainsKey(tweenHash))
            {
                _tweeners[tweenHash].Kill();
            }

            var tweener = BuildTweener(tween);
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

            tweener.SetId(_runningTweenId);
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
        
        public TweenDescription GetTween(int index)
        {
            return tweens[index];
        }
        
        
        public void SetTween(int index, TweenDescription tween)
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
            TweenDescription tween = tweens[idx];

            // if we arent going to delay anyway then need to wait until any parent layouts are done
            if (!tween.RandomDelay && Mathf.Approximately(tween.DefaultDelay.x, 0))
            {
                yield return new WaitForEndOfFrame();
            }

            yield return StartCoroutine(CoPlay(tweens[idx]));
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
                case Property.SpriteShaderFloat: return BuildShaderFloatTweener(tween);
                case Property.SpriteAlpha: return BuildSpriteAlphaTweener(tween);
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

        private Tweener BuildShaderFloatTweener(TweenDescription tween)
        {
            Material material;

            var sr = tween.ObjectToAnimate.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                material = sr.material;
            }
            else
            {
                var img = tween.ObjectToAnimate.GetComponent<Image>();

                if (img != null)
                {
                    material = img.material;
                }
                else
                {
                    Debug.LogError(
                        $"Object {tween.ObjectToAnimate.name} doesnt have a SpriteRenderer or an Image component");
                    return null;
                }
            }

            DOGetter<float> getter;
            float tweenStart;
            float tweenEnd;
            if (tween.RelativeToCurrent)
            {
                var initial = material.GetFloat(tween.PropertyName);
                getter = () => material.GetFloat(tween.PropertyName) + initial;
                tweenStart = tween.Start.x + initial;
                tweenEnd = tween.End.x + initial;
            }
            else
            {
                getter = () => material.GetFloat(tween.PropertyName);
                tweenStart = tween.Start.x;
                tweenEnd = tween.End.x;
            }

            SetShaderProperty(material, tween.PropertyName, tweenStart);

            // theres a weird bug where the property isnt being updated smoothly when timescale independant
            // it seems to jump frames ahead to nealy being done. cant see why though
            return DOTween.To(getter, it => SetShaderProperty(material, tween.PropertyName, it), tweenEnd, tween.Duration);
            // return DOTween
            //     .To(getter, it => SetShaderProperty(material, tween.PropertyName, it), tweenEnd, tween.Duration);
        }

        private void SetShaderProperty(Material material, string propertyName, float value)
        {
            material.SetFloat(propertyName, value);
        }

        private Tweener BuildSpriteAlphaTweener(TweenDescription tween)
        {
            var sr = tween.ObjectToAnimate.GetComponent<SpriteRenderer>();
            var tweenStart = tween.Start.x;
            var tweenEnd = tween.End.x;
            if (tween.RelativeToCurrent)
            {
                tweenStart += sr.color.a;
                tweenEnd += sr.color.a;
            }

            var c = sr.color;
            c.a = tweenStart;
            sr.color = c;
            return sr.DOFade(tweenEnd, tween.Duration).From(tweenStart);
        }

        #region debug extras
        private string TweenName(TweenDescription tween)
        {
            if (tween.ObjectToAnimate != null)
            {
                return tween.ObjectToAnimate.name;
            }
            else
            {
                return name;
            }
        }
        
        private string TweenToString(TweenDescription tween)
        {
            var sb = new StringBuilder();
            sb.AppendLine("TweenDescription:");
            sb.AppendLine($"ObjectToAimate: {TweenName(tween)}");
            sb.AppendLine($"Property: {tween.Property}");
            sb.AppendLine($"PropertyName: {tween.PropertyName}");
            sb.AppendLine($"Duration: {tween.Duration}");
            sb.AppendLine($"Start: {tween.Start}");
            sb.AppendLine($"End: {tween.End}");
            sb.AppendLine($"Ease: {tween.Ease}");
            sb.AppendLine($"PlayType: {tween.PlayType}");

            return sb.ToString();
        }
        #endregion
    }
}