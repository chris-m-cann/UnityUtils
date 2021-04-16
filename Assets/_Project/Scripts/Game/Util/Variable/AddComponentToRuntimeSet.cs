using System;
using UnityEngine;

namespace Util.Variable
{
    public class AddComponentToRuntimeSet<T> : MonoBehaviour
    {
        [SerializeField] private RuntimeSet<T> set;

        private bool _inSet = false;

        private T _component;
        private void Awake()
        {
            _component = GetComponent<T>();
            AddToSet();
        }

        public void AddToSet()
        {
            if (!_inSet && _component != null && set != null)
            {
                _inSet = set.Add(_component);
            }
        }

        public void Remove()
        {
            if (_inSet && _component != null && set != null)
            {
                set.Remove(_component);
                _inSet = false;
            }
        }

        private void OnDestroy()
        {
            Remove();
        }
    }
}