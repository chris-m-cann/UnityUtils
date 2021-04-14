using UnityEngine;
using UnityEngine.Events;

namespace Util.Events
{
    public abstract class GameEventListenerBehaviour<T, TGameEvent> : MonoBehaviour where TGameEvent : GameEvent<T>
    {
        [SerializeField] private TGameEvent gameEvent;

        [SerializeField] private UnityEvent<T> onEventRaised;


        private void OnEnable()
        {
            if (gameEvent == null) return;
            gameEvent.OnEventTrigger += OnEventRaised;
        }

        private void OnDisable()
        {
            if (gameEvent == null) return;
            gameEvent.OnEventTrigger -= OnEventRaised;
        }

        public void OnEventRaised(T t)
        {
            onEventRaised.Invoke(t);
        }
    }
}