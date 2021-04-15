using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Util
{
    public static class CoroutinesUtils
    {

        public static void ExecuteAfter(this MonoBehaviour self, float delay, UnityAction action) {
            self.StartCoroutine(CoExecuteAfter(delay, action, true));
        }

        public static void ExecuteAfterUnscaled(this MonoBehaviour self, float delay, UnityAction action)
        {
            self.StartCoroutine(CoExecuteAfter(delay, action, false));
        }

        private static IEnumerator CoExecuteAfter(float delay, UnityAction action, bool useTimeScale)
        {
            if (useTimeScale)
            {
                yield return new WaitForSeconds(delay);
            } else
            {
                yield return new WaitForSecondsRealtime(delay);
            }

            action();
        }

    }
}
