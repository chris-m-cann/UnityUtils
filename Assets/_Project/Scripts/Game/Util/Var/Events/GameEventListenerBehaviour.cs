using UnityEngine;
using UnityEngine.Events;

namespace Util.Var.Events
{
    public abstract class GameEventListenerBehaviour<T, TGameEvent> : MonoBehaviour where TGameEvent : EventReferenceBase<T>
    {
        [SerializeField] private TGameEvent gameEvent;

        [SerializeField] private UnityEvent<T> onEventRaised;


        private void OnEnable()
        {
            if (gameEvent == null) return;
            gameEvent.OnEventTriggered += OnEventRaised;
        }

        private void OnDisable()
        {
            if (gameEvent == null) return;
            gameEvent.OnEventTriggered -= OnEventRaised;
        }

        public void OnEventRaised(T t)
        {
            onEventRaised.Invoke(t);
        }
    }
}