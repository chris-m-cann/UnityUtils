using UnityEngine;

namespace Util
{
    // todo(chris) bit of a naff name give the existing range attriute
    public class RangeSliderAttribute : PropertyAttribute
    {
        public readonly float Min;
        public readonly float Max;

        public RangeSliderAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}