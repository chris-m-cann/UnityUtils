using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Util.Var
{
    [InlineEditor]
    public class Variable<T>: ScriptableObject
    {
        [SerializeField] protected T value;
        [SerializeField] protected bool persistValue = true;
        [ShowIf("@!persistValue")]
        [SerializeField] protected T resetValue;

        public virtual T Value {
            get => value;
            set
            {
                this.value = value;
            }
        }

        private void OnEnable()
        {
            if (!persistValue)
            {
                value = resetValue;
            }
        }

        private void OnDisable()
        {
            if (!persistValue)
            {
                value = resetValue;
            }
        }

        public void Set(T newValue) => Value = newValue;

        // This takes a ScriptableObject rather than a Variable<T> 
        // for the sake of the unity editor
        // functions that take generic parameters dont work
        // in unity events
        public void CopyValue(ScriptableObject newValue)
        {
            if (newValue is Variable<T> variable)
            {
                Value = variable.Value;
            }
            else
            {
                Debug.LogError($"expected type {this.GetType().Name}, instead got {newValue.GetType().Name}");
            }
        }
        public T Get() => Value;

        public void Reset()
        {
            Value = resetValue;
        }
    }
}