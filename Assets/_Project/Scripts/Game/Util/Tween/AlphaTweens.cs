using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Util.Tween
{
    public class SpriteAlphaTween : TweenBase<float, SpriteRenderer>
    {
        protected override float RelativeStart => Start + Target.color.a;
        protected override float RelativeEnd => End + Target.color.a;

        protected override Tweener BuildTweener(float tweenStart, float tweenEnd)
        {
            return Target.DOFade(tweenEnd, Duration).From(tweenStart);            
        }
    }
    
    public class CanvasGroupAlphaTween : TweenBase<float, CanvasGroup>
    {
        protected override float RelativeStart => Start + Target.alpha;
        protected override float RelativeEnd => End + Target.alpha;
        
        protected override Tweener BuildTweener(float tweenStart, float tweenEnd)
        {
            return Target.DOFade(tweenEnd, Duration).From(tweenStart);
        }
    }
}