using UnityEngine;

namespace Util.Events
{
    [CreateAssetMenu(menuName = "Custom/Events/Void")]
    public class VoidGameEvent : GameEvent<Void>
    {
        public void Raise() => Raise(Void.Instance);
    }
}