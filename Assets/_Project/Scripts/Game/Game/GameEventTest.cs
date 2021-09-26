using Sirenix.OdinInspector;
using UnityEngine;
using Util.Var;
using Util.Var.Events;
using Util.Var.Observe;

namespace Game
{
    public class GameEventTest : MonoBehaviour
    {
        [SerializeField] private VoidGameEvent voidGameEvent;
        [SerializeField] private IntEventReference intEvent;
        [SerializeField] private StringReference stringReference;
    }
}