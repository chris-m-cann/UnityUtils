using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Util.Var.Events
{
    [InlineEditor]
    public class GameEvent<T> : ScriptableObject
    {
        public event Action<T> OnEventTrigger;


#if UNITY_EDITOR
        [HideIf("@IsVoid")]
        public T Value;

        private bool IsVoid => typeof(T) == typeof(Void);
        
        [Button, EnableIf("@UnityEngine.Application.isPlaying")]
        public void Raise() => Raise(Value);
#endif
        public void Raise(T t) => OnEventTrigger?.Invoke(t);
    }
}