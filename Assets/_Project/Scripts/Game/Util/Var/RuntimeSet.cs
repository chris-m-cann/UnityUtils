using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Util.Var
{
    public class RuntimeSet<T> : ScriptableObject
    {
        [NonSerialized] protected List<T> items = new List<T>();

        public ReadOnlyCollection<T> ListView => items.AsReadOnly();
        public bool IsEmpty => items.Count == 0;

        public virtual bool Add(T thing)
        {
            if (!items.Contains(thing))
            {
                items.Add(thing);
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool Remove(T thing)
        {
            if (items.Contains(thing))
                return items.Remove(thing);

            return false;
        }

        public virtual bool RemoveAt(int idx)
        {
            if (idx > -1 && idx < items.Count)
            {
                items.RemoveAt(idx);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ForEach(Action<T> block)
        {
            foreach (var item in items)
            {
                block.Invoke(item);
            }
        }

        public virtual void Clear() => items.Clear();
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }

    }
}