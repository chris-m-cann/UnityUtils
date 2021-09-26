using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Util.Var.Observe;

namespace Util.Var
{
    [Serializable]
    public class VariableReference<TVar, TObVar, T> where TVar : Variable<T> where TObVar : ObservableVariable<T>
    {
        
        [HorizontalGroup("GroupName", 90), HideLabel]
        [ValueDropdown("GetValues")]
        [SerializeField] protected int Delimeter;
        
        [HorizontalGroup("GroupName"), HideLabel]
        [ShowIf("@Delimeter == 0")]
        [SerializeField] private TVar Variable;
        
        [HorizontalGroup("GroupName"), HideLabel]
        [ShowIf("@Delimeter == 1")]
        [SerializeField] private TObVar Observable;
        
        [HorizontalGroup("GroupName"), HideLabel]
        [ShowIf("@Delimeter == 2")]
        [SerializeField] private T Constant;

        protected virtual ValueDropdownList<int> GetValues()
        {
            return new ValueDropdownList<int>()
            {
                {"Variable", 0},
                {"Observable", 1},
                {"Constant", 2},
            };
        }
        
        

        public virtual T Value
        {
            get
            {
                switch (Delimeter)
                {
                    case 0: return Variable.Value;
                    case 1: return Observable.Value;
                    default: return Constant;
                }
            }

            set
            {
                switch (Delimeter)
                {
                    case 0: Variable.Value = value;
                        return;
                    case 1: Observable.Value = value;
                        return;
                    default: Constant = value;
                        return;
                }
            }
        }
    }
}