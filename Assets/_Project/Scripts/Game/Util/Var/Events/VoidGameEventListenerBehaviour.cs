using UnityEngine;
using UnityEngine.Events;

namespace Util.Var.Events
{
    public class VoidGameEventListenerBehaviour : MonoBehaviour
    {
        [SerializeField] private VoidGameEvent gameEvent;

        [SerializeField] private UnityEvent onEventRaised;


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

        public void OnEventRaised(Void v)
        {
            onEventRaised.Invoke();
        }
    }
}