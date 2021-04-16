using System;
using UnityEngine;

namespace Util.Variable
{
    public class ResetVariables<T> : MonoBehaviour
    {
        [SerializeField] private ResetTime time;

        [SerializeField] private Pair<Variable<T>, T>[] vars;

        private void Awake()
        {
            if (time != ResetTime.OnAwake) return;
            Reset();
        }

        private void Start()
        {
            if (time != ResetTime.OnStart) return;
            Reset();
        }

        private void OnEnable()
        {
            if (time != ResetTime.OnEnable) return;
            Reset();
        }

        private void OnDisable()
        {
            if (time != ResetTime.OnDisable) return;
            Reset();
        }

        private void OnDestroy()
        {
            if (time != ResetTime.OnDestroy) return;
            Reset();
        }

        public void Reset()
        {
            foreach (var (variable, value) in vars)
            {
                variable.Value = value;
            }
        }
    }

    public enum ResetTime
    {
        OnAwake, OnStart, OnEnable, OnDisable, OnDestroy
    }
}