using System;
using UnityEngine;

namespace Util.Events
{
    public class GameEvent<T> : ScriptableObject
    {
        public event Action<T> OnEventTrigger;

#if UNITY_EDITOR
        public T ValueToRaise;
#endif
        private void OnEnable()
        {
            if (OnEventTrigger == null)
            {
                OnEventTrigger += NullOp.Fun;
            }
        }

        public void Raise(T t)
        {
            OnEventTrigger.Invoke(t);
        }
    }
}