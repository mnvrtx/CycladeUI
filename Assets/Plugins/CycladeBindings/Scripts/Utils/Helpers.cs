using System;
using UnityEngine;

namespace CycladeBindings.Utils
{
    public static class Helpers
    {
        public static void IterateOnMeAndChildren<T>(this Component o, Action<T> action) where T : Component
            => IterateOnMeAndChildren(o.gameObject, action);

        public static void IterateOnMeAndChildren<T>(this GameObject o, Action<T> applyAct) where T : Component
        {
            foreach (var child in o.GetComponentsInChildren<T>())
                applyAct.Invoke(child);
        }

        public static string GetFullPath(this GameObject go, int max = -1, bool skipFirst = false)
        {
            var transform = go.transform;

            string path = !skipFirst ? transform.name : "";
            int i = 0;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
                i++;

                if (max != -1)
                {
                    if (i == max)
                    {
                        path = $".../{path}";
                        break;
                    }
                }
            }

            return path;
        }
    }
}