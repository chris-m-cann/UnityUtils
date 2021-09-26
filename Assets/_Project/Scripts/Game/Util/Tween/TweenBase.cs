using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Util.Tween
{
    public abstract class TweenBase<TValue, TTarget> : Tween where TTarget : Component
    {
        [PropertyOrder(-1), Tooltip("If this is null then will try and find component in parent object")]
        public TTarget Target;
        public TValue Start;
        public TValue End;
        protected abstract TValue RelativeStart { get; }
        protected abstract TValue RelativeEnd { get; }

        public override Tweener BuildTweener(GameObject parent)
        {
            Init(parent);
            
            var tweenStart = Start;
            var tweenEnd = End;
            if (RelativeToCurrent)
            {
                tweenStart = RelativeStart;
                tweenEnd = RelativeEnd;
            }

            return BuildTweener(tweenStart, tweenEnd);
        }

        protected virtual void Init(GameObject parent)
        {
            if (Target == null)
            {
                Target = parent.GetComponent<TTarget>();
            }
        }
        protected abstract Tweener BuildTweener(TValue tweenStart, TValue tweenEnd);
    }
}