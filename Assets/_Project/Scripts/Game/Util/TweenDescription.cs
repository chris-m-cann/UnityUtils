using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Util
{
        [System.Serializable]
        public struct TweenDescription
        {
            public string Name;
            [Tooltip("If null, object this component is attached to will be used")]
            public GameObject ObjectToAnimate;

            public Ease Ease;
            public bool UseCustomCurve;
            public AnimationCurve CustomCurve;
            public TweenBehaviour.Property Property;
            public string PropertyName;
            public float Duration;
            public bool RelativeToCurrent;
            public Vector3 Start;
            public Vector3 End;
            public TweenBehaviour.PlayType PlayType;
            public bool PlayOnEnable;
            public bool RandomDelay;
            public Vector2 DefaultDelay;
            public bool TimeScaleDependent;
            public UnityEvent OnComplete;
        }
}