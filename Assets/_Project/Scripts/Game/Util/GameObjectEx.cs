using UnityEngine;

namespace Util
{
    public static class GameObjectEx
    {
        public static T GetComponentIfNull<T>(this GameObject self, T current)
        {
            if (current == null)
            {
                return self.GetComponent<T>();
            }
            else
            {
                return current;
            }
        }

        public static T GetComponentIfNull<T>(this Component self, T current)
        {
            return self.gameObject.GetComponentIfNull(current);
        }
    }
}