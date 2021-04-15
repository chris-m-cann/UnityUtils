using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public static class RandomEx
    {
        #region ranges

        /*
         *  Ranges:
         *   [ - includes
         *   ( - excludes
         *
         *   [start, end] - Closed Range
         *   (start, end) - Open Range
         *   [start, end) - Half-Open Range
         *   (start, end] - half-Open Range (I'll call it half-closed to differentiate)
         *
         *   minDiff - the closest you can get to a point without being "equal" to it
         */

        public static float Range(Range range) => ClosedRange(range);
        public static float ClosedRange(Range range) => Random.Range(range.Start, range.End);
        public static float OpenRange(Range range, float minDiff = float.Epsilon) => Random.Range(range.Start + minDiff, range.End - minDiff);
        public static float HalfOpenRange(Range range, float minDiff = float.Epsilon) => Random.Range(range.Start, range.End - minDiff);
        public static float HalfClosedRange(Range range, float minDiff = float.Epsilon) => Random.Range(range.Start + minDiff, range.End);

        #endregion


        public static T RandomElement<T>(this T[] self) => self[Random.Range(0, self.Length)];
        public static T RandomElement<T>(this List<T> self) => self[Random.Range(0, self.Count)];
    }
}