using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Util.Tween
{
    public abstract class MaterialFloatTween<T> : TweenBase<float, T> where T : Component
    {
        [PropertyOrder(-1)]
        public string PropertyName;
        
        protected Material material;
        protected override float RelativeStart => Start + GetCurrent();
        protected override float RelativeEnd => End + GetCurrent();

        private float GetCurrent()
        {
            return material.GetFloat(PropertyName);
        }

        private void SetValue(float value)
        {
            material.SetFloat(PropertyName, value);
        }

        protected override Tweener BuildTweener(float tweenStart, float tweenEnd)
        {
            return DOTween.To(GetCurrent, SetValue, tweenEnd, Duration).From(tweenStart);
        }
    }

    public class RendererMaterialFloatTween : MaterialFloatTween<Renderer>
    {
        protected override void Init(GameObject parent)
        {
            base.Init(parent);
            material = Target.material;
        }
    }
    
    public class ImageMaterialFloatTween : MaterialFloatTween<MaskableGraphic>
    {
        protected override void Init(GameObject parent)
        {
            base.Init(parent);
            material = Target.material;
        }
    }
}