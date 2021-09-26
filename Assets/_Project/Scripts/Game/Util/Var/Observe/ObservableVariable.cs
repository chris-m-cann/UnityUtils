using System;
using Sirenix.OdinInspector;
using Util.Var.Events;

namespace Util.Var.Observe
{
    public class ObservableVariable<T> : Variable<T>
    {
        public event Action<T> OnValueChanged;

        public event Action<T> OnEventTrigger
        {
            add => OnValueChanged += value;
            remove => OnValueChanged -= value;
        }

        public override T Value
        {
            get => base.Value;
            set
            {
                if (base.Value?.Equals(value) == true) return;

                base.Value = value;
                Raise(value);
            }
        }
        
        
        [Button, EnableIf("@UnityEngine.Application.isPlaying")]
        public void Raise()
        {
            Raise(Value);
        }

        public void Raise(T t)
        {
            OnValueChanged?.Invoke(t);
        }
    }
}