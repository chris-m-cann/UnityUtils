using UnityEngine;

namespace Util.Var.Events
{
    [CreateAssetMenu(menuName = "Custom/Events/Void")]
    public class VoidGameEvent : GameEvent<Void>
    {
        public void Raise() => Raise(Void.Instance);
    }
}