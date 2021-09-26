using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Util
{
    public static class EditorEx
    {
        public static bool ModifyObject(this SerializedObject self, Action block)
        {
            bool changed = false;
            try
            {
                self.Update();

                block();
            }
            finally
            {
                changed = self.ApplyModifiedProperties();
            }

            return changed;
        }

        public class ModifyObjectScope : IDisposable
        {
            private SerializedObject _so;

            public ModifyObjectScope(SerializedObject so)
            {
                _so = so;
                _so.Update();
            }

            public void Dispose()
            {
                _so.ApplyModifiedProperties();
            }
        }

        public static IDisposable UpdateScope(this SerializedObject self)
        {
            return new ModifyObjectScope(self);
        }

        
        // if left mouse clicked then remove focus from our properties
        public static void RemoveFocusOnMouseDown(this EditorWindow self)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                GUI.FocusControl(null);
                self.Repaint();
            }
        }
        
        // iterate over the top level fields of an object
        public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty self)
        {
            // figure out the end iterator by taking a copy of current position and
            // attempting to move to next memeber of iverall object
            // i.e. if we get to end then weve finished iterating over self's children
            var end = self.Copy();
            var hasNextElement = end.NextVisible(false);
            // if parent object has no more children then = null
            if (!hasNextElement)
            {
                end = null;
            }

            // get an independent iterator pointing to the fist child of self
            var it = self.Copy();
            it.NextVisible(true);

            while (true)
            {
                // if we are equal to the end then we are done
                if (SerializedProperty.EqualContents(it, end))
                {
                    yield break;
                }
                
                // yeild the iterator to the child (copy so noone skrews with it)
                yield return it.Copy();

                // move to next child of the self property but not into any of those children's children
                if (!it.NextVisible(false))
                {
                    break; // if there are no more children then end
                }
            }
        }
    }
}