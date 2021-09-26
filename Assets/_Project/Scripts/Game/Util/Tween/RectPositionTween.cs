using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Util.Tween
{
    public class RectPositionTween : TweenBase<Vector2, RectTransform>
    {
        protected override Vector2 RelativeStart => Start + Target.anchoredPosition;
        protected override Vector2 RelativeEnd => End + Target.anchoredPosition;

        protected override Tweener BuildTweener(Vector2 tweenStart, Vector2 tweenEnd)
        {
           return DOTween.To(() => Target.anchoredPosition, it => Target.anchoredPosition = it, tweenEnd, Duration).From(tweenStart);
        }
    }
}