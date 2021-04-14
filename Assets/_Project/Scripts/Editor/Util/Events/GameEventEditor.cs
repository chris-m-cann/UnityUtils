using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Util.Events
{

    public class GameEventEditor<T> : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var prev = GUI.enabled;
            GUI.enabled = EditorApplication.isPlaying;
            if (GUILayout.Button("Raise"))
            {
                var @event = ((GameEvent<T>) target);
                @event.Raise(@event.ValueToRaise);
            }

            GUI.enabled = prev;
        }
    }
}