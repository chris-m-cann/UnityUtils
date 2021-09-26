using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Util.Tween
{
    public class PositionTween : TweenBase<Vector3, Transform>
    {
        protected override Vector3 RelativeStart => Start + Target.position;
        protected override Vector3 RelativeEnd => End + Target.position;

        protected override Tweener BuildTweener(Vector3 tweenStart, Vector3 tweenEnd)
        {
            return Target.DOMove(tweenEnd, Duration).From(tweenStart);
        }
    }

    public class ScaleTween : TweenBase<Vector3, Transform>
    {
        protected override Vector3 RelativeStart => Start + Target.localScale;
        protected override Vector3 RelativeEnd => End + Target.localScale;

        protected override Tweener BuildTweener(Vector3 tweenStart, Vector3 tweenEnd)
        {
            return Target.DOScale(tweenEnd, Duration).From(tweenStart);
        }
    }

    public class RotationTween : TweenBase<Vector3, Transform>
    {
        protected override Vector3 RelativeStart => Start + Target.eulerAngles;
        protected override Vector3 RelativeEnd => End + Target.eulerAngles;

        protected override Tweener BuildTweener(Vector3 tweenStart, Vector3 tweenEnd)
        {
            return Target.DORotate(tweenEnd, Duration).From(tweenStart);
        }
    }
}