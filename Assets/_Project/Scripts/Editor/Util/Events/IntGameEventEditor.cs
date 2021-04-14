using UnityEditor;
using UnityEngine;

namespace Util.Events
{
    [CustomEditor(typeof(IntGameEvent))]
    [CanEditMultipleObjects]
    public class IntGameEventEditor : GameEventEditor<int>
    {
    }
}