using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Util.Var.Observe;

namespace Util.Var.Events
{
    public abstract class EventReferenceBase<T>
    {
        public abstract event Action<T> OnEventTriggered;
        public abstract void Raise(T t);
    }

    public class EventReference<TEvent, TObVar, T> : EventReferenceBase<T> where TEvent : GameEvent<T> where TObVar : ObservableVariable<T>
    {
                
        [HorizontalGroup("Group", 90), HideLabel]
        [ValueDropdown("GetValues")]
        [SerializeField] protected int Delimeter;
        
        [HorizontalGroup("Group"), HideLabel]
        [ShowIf("@Delimeter == 0")]
        [SerializeField] protected TEvent Event;
        
        [HorizontalGroup("Group"), HideLabel]
        [ShowIf("@Delimeter == 1")]
        [SerializeField] protected  TObVar Variable;
        
        protected virtual ValueDropdownList<int> GetValues()
        {
            return new ValueDropdownList<int>()
            {
                {"Event", 0},
                {"Observable", 1}
            };
        }

        public override event Action<T> OnEventTriggered
        {
            add
            {
                if (Delimeter == 0) Event.OnEventTrigger += value;
                else Variable.OnValueChanged += value;
            }
            remove
            {
                if (Delimeter == 0) Event.OnEventTrigger -= value;
                else Variable.OnValueChanged -= value;
            }
        }

        public override void Raise(T t)
        {
            if (Delimeter == 0) Event.Raise(t);
            else Variable.Raise(t);
        }

    }
}