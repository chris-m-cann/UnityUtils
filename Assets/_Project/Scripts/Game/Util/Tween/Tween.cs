using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Util.Tween
{
    public abstract class Tween
    {
        [PropertyOrder(-2), Tooltip("Just a description for you to differentiate. not used in code")]
        public string Name;
        
        // basic settings
        public float Duration = 1f;
        public TweenBehaviour.PlayType PlayType;
        public bool TimeScaleDependent = true;
        public bool PlayOnEnable;

        // easing settings
        [LabelText("Easing")]
        [HorizontalGroup("Easing", 280), ValueDropdown("EasingOptions")]
        public bool UseCustomCurve;
        
        [HorizontalGroup("Easing"), HideLabel]
        [ShowIf("@!UseCustomCurve")]
        public Ease Ease = Ease.Linear;
        
        [HorizontalGroup("Easing"), HideLabel]
        [ShowIf("@UseCustomCurve"), DrawWithUnity]
        public AnimationCurve CustomCurve;

        private static ValueDropdownList<bool> EasingOptions = new ValueDropdownList<bool>()
        {
            {"EasingType", false},
            {"Custom Curve", true}
        };


        // initial delay settings
        [LabelText("Delay")]
        [HorizontalGroup("Delay", 280), ValueDropdown("DelayOptions")]
        public bool RandomDelay;
        
        [HorizontalGroup("Delay"), HideLabel]
        [ShowIf("@!RandomDelay")]
        public float Delay;
        
        [HorizontalGroup("Delay"), HideLabel]
        [ShowIf("@RandomDelay")]
        public Vector2 DelayRange;
        
        
        private static ValueDropdownList<bool> DelayOptions = new ValueDropdownList<bool>()
        {
            {"Constant", false},
            {"Random Range", true}
        };
        
        
        // should appear just above the start and end this way
        public bool RelativeToCurrent;
        
        
        [DrawWithUnity, PropertyOrder(999)] 
        public UnityEvent OnComplete;
        
        
        public abstract Tweener BuildTweener(GameObject parent);
    }
}