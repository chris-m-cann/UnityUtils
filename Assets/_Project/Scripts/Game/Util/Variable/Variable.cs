using System;
using UnityEngine;

namespace Util.Variable
{
    public class Variable<T>: ScriptableObject
    {
        [SerializeField] private T value;

        public virtual T Value {
            get => value;
            set => this.value = value;
        }

        public void Set(T newValue) => Value = newValue;
        public T Get() => Value;
    }
}