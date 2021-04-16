using System;
using UnityEngine;

namespace Util.Variable
{
    public class ObservableVariable<T> : Variable<T>
    {
        public event Action<T> OnValueChanged;

        public override T Value
        {
            get => base.Value;
            set
            {
                if (base.Value.Equals(value)) return;

                base.Value = value;
                Raise(value);
            }
        }


        public void Raise(T t)
        {
            OnValueChanged?.Invoke(t);
        }
    }
}