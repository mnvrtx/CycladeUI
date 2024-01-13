using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CycladeBase.Utils
{
    public static class CycladeHelpers
    {
        public static Color ParseHex(string hex)
        {
            hex = hex.TrimStart('#');

            Color col;
            if (hex.Length == 6)
            {
                col = new Color( // hardcoded opaque
                    int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber) / 255f,
                    int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber) / 255f,
                    int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber) / 255f,
                    1);
            }
            else
            {
                col = new Color(
                    int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber) / 255f,
                    int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber) / 255f,
                    int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber) / 255f,
                    int.Parse(hex.Substring(6, 2), NumberStyles.HexNumber) / 255f);
            }

            return col;
        }

        public static void ToInitial(this RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.one / 2;
            rectTransform.anchorMax = Vector2.one / 2;
            rectTransform.pivot = Vector2.one / 2;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.rotation = Quaternion.identity;
        }

        public static void StretchAcrossParent(this RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        public static T Pop<T>(this List<T> list)
        {
            var lastEl = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return lastEl;
        }

        public static void SetActive(this Component comp, bool value)
        {
            comp.gameObject.SetActive(value);
        }

        public static string Bold(this string text, bool isNeed = true) => !isNeed ? text : $"<b>{text}</b>";
        public static string ColorMark(this string text, bool isNeed = true, string color = "orange") => !isNeed ? text : $"<color={color}>{text}</color>";

        public static Dictionary<TValue, TKey> SwapKeysAndValues<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return dictionary.ToDictionary(t => t.Value, t => t.Key);
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

        public static List<Type> FindTypesWith(Func<Type, bool> predicate)
        {
            var assemblies = FindAssembliesWith(predicate);

            var list = new List<Type>();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types.Where(predicate))
                    list.Add(type);
            }

            return list;
        }

        public static Assembly[] FindAssembliesWith(Func<Type, bool> predicate)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies
                .Where(a => a.GetTypes().Any(predicate))
                .ToArray();
        }

        public static string ConvertToAssetPath(string fullPath)
        {
            string projectPath = Path.GetDirectoryName(Application.dataPath);

            // ReSharper disable once AssignNullToNotNullAttribute
            if (!fullPath.StartsWith(projectPath))
            {
                Debug.LogError("The provided path is not part of the Unity project.");
                return null;
            }

            string relativePath = fullPath.Substring(projectPath.Length + 1).Replace('\\', '/');
            return relativePath;
        }

        public static int GetIndexOfComponent(Component comp)
        {
            var components = comp.gameObject.GetComponents<Component>();
            return components.ToList().IndexOf(comp);
        }

        public static Component FindComponent(GameObject go, Func<Component, bool> func, out Component[] foundComponents)
        {
            foundComponents = go.GetComponents<Component>();
            Component foundComponent = default;
            foreach (var component in foundComponents)
            {
                if (func.Invoke(component))
                {
                    foundComponent = component;
                    break;
                }
            }

            return foundComponent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ValidIndex<T>(this T[] arr, int idx) => arr != null && idx > -1 && idx < arr.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ValidIndex<T>(this IList<T> list, int idx) => list != null && idx > -1 && idx < list.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TryGet<T>(this IList<T> list, int idx) => list != null && idx > -1 && idx < list.Count ? list[idx] : default;

        public static void Set<T>(this IList<T> list, int idx, T data)
        {
            var extendCount = idx - list.Count + 1;
            if (extendCount > 0)
            {
                for (int i = 0; i < extendCount; i++)
                    list.Add(default);
            }

            list[idx] = data;
        }

        public static void SetRectTransformValues(RectTransform toRt, RectTransform referenceRt)
        {
            toRt.anchorMin = referenceRt.anchorMin;
            toRt.anchorMax = referenceRt.anchorMax;
            toRt.rotation = referenceRt.rotation;
            toRt.pivot = referenceRt.pivot;
            toRt.position = referenceRt.position;
            toRt.localScale = referenceRt.localScale;
            toRt.sizeDelta = referenceRt.sizeDelta;
        }

        public static T GetPrivateOrOtherField<T>(this object obj, string name)
        {
            // Set the flags so that private and public fields from instances will be found
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = obj.GetType().GetField(name, bindingFlags);
            return (T)field?.GetValue(obj);
        }

        public static T GetPrivateOrOtherProp<T>(this object obj, string name)
        {
            // Set the flags so that private and public fields from instances will be found
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var prop = obj.GetType().GetProperty(name, bindingFlags);
            return (T)prop?.GetValue(obj);
        }

        public static void IterateOverMeAndChildren(GameObject obj, Action<GameObject> iterationAct, Func<GameObject, bool> needToGoDeepFunc)
        {
            iterationAct.Invoke(obj);

            if (!needToGoDeepFunc.Invoke(obj))
                return;

            foreach (Transform child in obj.transform)
                IterateOverMeAndChildren(child.gameObject, iterationAct, needToGoDeepFunc);
        }

        private static readonly string[] sizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

        public static string SizeSuffix(this long byteCount)
        {
            if (byteCount == 0)
                return "0" + sizeSuffixes[0];
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return Math.Sign(byteCount) * num + sizeSuffixes[place];
        }
        
        public const float BEpsilon = 0.00001f * 8;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(float a, float b) => Abs(b - a) < Max(1E-06f * Max(Abs(a), Abs(b)), BEpsilon);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(float f) => Math.Abs(f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float a, float b) => (a <= b) ? b : a;
        
        public static Color WithAlpha(this Color color, float a) 
            => new(color.r, color.g, color.b, a);

        public static bool EqualsWithoutAlpha(this Color c, Color other) 
            => c.r.Equals(other.r) && c.g.Equals(other.g) && c.b.Equals(other.b);

        public static void FastRemoveAndDecrI<T>(this List<T> list, ref int i)
        {
            list.FastRemoveAt(i);
            i--;
        }
        
        public static void FastRemoveAt<T>(this List<T> list, int i)
        {
            list[i] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
        }
    }
}