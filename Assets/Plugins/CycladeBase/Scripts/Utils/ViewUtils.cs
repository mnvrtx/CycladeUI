using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Shared.Utils;
using Shared.Utils.Logging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Solonity.View.Utils
{
    public static class ViewUtils
    {
        private static readonly Log log = new(nameof(ViewUtils), CycladeDebugInfo.I);

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
        
        public static Color WithAlpha(this Color color, float a) 
            => new(color.r, color.g, color.b, a);

        public static bool EqualsWithoutAlpha(this Color c, Color other) 
            => c.r.Equals(other.r) && c.g.Equals(other.g) && c.b.Equals(other.b);

        public static List<GameObject> GetAllParents(this Transform t)
        {
            var result = new List<GameObject>();

            var parent = t.parent;
            while (parent != null)
            {
                result.Add(parent.gameObject);
                parent = parent.parent;
            }

            return result;
        }

        public static List<GameObject> GetAllParents(this GameObject obj)
        {
            return obj.transform.GetAllParents();
        }

        public static List<GameObject> GetAllParentsAndThis(this GameObject obj)
        {
            var result = new List<GameObject> { obj };
            result.AddRange(obj.transform.GetAllParents());
            return result;
        }

        public static void DestroyAllChildren(this Transform t)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                var child = t.GetChild(i);
                child.gameObject.Destroy();
                i--;
            }
        }

        public static void Destroy(this Object o, bool allowDestroyAssets = false)
        {
            if (o == null)
                return;

            if (Application.isPlaying)
                Object.Destroy(o);
            else
                Object.DestroyImmediate(o, allowDestroyAssets);
        }

        public static void DestroyGameObject(this Component o, bool allowDestroyAssets = false)
        {
            if (Application.isPlaying)
                Object.Destroy(o.gameObject);
            else
                Object.DestroyImmediate(o.gameObject, allowDestroyAssets);
        }
        
        public static void DestroyAllChildren(this Component o)
        {
            var go = o.gameObject;
            foreach (Transform child in go.transform) 
                GameObject.Destroy(child.gameObject);
        }

        public static IEnumerable<Transform> GetChildren(this Transform o) => o.Cast<Transform>();

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

        public static string FormatToPercent(this float value)
        {
            return $"{(int)(value * 100)}%";
        }

        public static string GetCurrencyIconCode(bool isCyberbucks)
        {
            string currencySp;
            if (isCyberbucks)
            {
                currencySp = "<sprite=4>";
            }
            else
            {
                currencySp = "<sprite=5>";
            }

            return currencySp;
        }

        public static string ToPrettyCurrency(this int val)
        {
            return $"{val:N0}"; //// val, 12445 -> Displays 12,445
        }

        public static string ToPrettyVal(this int val)
        {
            return $"{val:N0}"; //// val, 12445 -> Displays 12,445
        }

        public static void DestroyAllChildren(this GameObject obj)
        {
            obj.transform.DestroyAllChildren();
        }

        public static void DestroyAllChildrenWhere(this GameObject obj, Func<GameObject, bool> filter)
        {
            var listToRemove = new List<GameObject>(obj.transform.childCount);
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var t = obj.transform.GetChild(i).gameObject;
                if (filter.Invoke(t))
                {
                    listToRemove.Add(t);
                }
            }

            foreach (var go in listToRemove)
            {
                go.Destroy();
            }
        }

        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                obj.transform.GetChild(i).gameObject.SetLayerRecursively(layer);
            }
        }

        public static List<GameObject> FindRecursive(this GameObject go, Func<Transform, bool> func)
        {
            var result = new List<GameObject>();
            FindRecursive(go, result, func);
            return result;
        }

        public static void FindRecursive(this GameObject go, List<GameObject> result, Func<Transform, bool> func)
        {
            foreach (Transform t in go.transform)
            {
                if (func.Invoke(t))
                    result.Add(t.gameObject);

                if (t.childCount > 0)
                    t.gameObject.FindRecursive(result, func);
            }
        }


        public static IEnumerator DeactivateWithDelay(this GameObject go, int duration)
        {
            yield return new WaitForSeconds(duration / 1000f);
            go.SetActive(false);
        }
        
        public static Transform Instantiate(this GameObject prefab, Transform parent = null)
        {
            var instance = GameObject.Instantiate(prefab, parent);
            instance.transform.SetToDefault();
            return instance.transform;
        }

        public static T Instantiate<T>(this T prefab, Transform parent = null) where T : Component
        {
            var instance = GameObject.Instantiate(prefab, parent);
            instance.transform.SetToDefault();
            return instance;
        }
        
        
        public static T InstantiateRectTransform<T>(this T prefab, Transform parent = null) where T : Component
        {
            var instance = GameObject.Instantiate(prefab, parent);
            var rt = instance.GetComponent<RectTransform>();
            if (rt != null) 
                rt.SetParentImitate();
            return instance;
        }

        public static void SetActive(this Component comp, bool value)
        {
            comp.gameObject.SetActive(value);
        }

        public static void SetActiveAll(this IEnumerable<GameObject> list, bool value)
        {
            foreach (var elem in list)
                elem.SetActive(value);
        }

        public static void SetActiveAll<T>(this IEnumerable<T> list, bool value) where T : Component
        {
            foreach (var elem in list)
                elem.SetActive(value);
        }

        public static void ClearAndDestroy(this List<GameObject> list)
        {
            foreach (var elem in list)
            {
                elem.Destroy();
            }

            list.Clear();
        }

        public static void ClearAndDestroy<T>(this List<T> list) where T : Component
        {
            foreach (var elem in list)
            {
                elem.gameObject.Destroy();
            }

            list.Clear();
        }

        public static RectTransform GetRect(this GameObject go) => go.GetComponent<RectTransform>();
        public static RectTransform GetRect(this Component m) => m.GetComponent<RectTransform>();

        public static void SetParentImitate(this RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        public static void SetToInitial(this RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.one / 2;
            rectTransform.anchorMax = Vector2.one / 2;
            rectTransform.pivot = Vector2.one / 2;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.rotation = Quaternion.identity;
        }

        public static EventTrigger AddTriggersEvents(Selectable button, EventTriggerType eventTriggerType, Action<BaseEventData> onTriggerAction = null)
        {
            EventTrigger eventrTrigger = button.gameObject.AddComponent<EventTrigger>();
            if (onTriggerAction != null)
            {
                EventTrigger.Entry pointerEvent = new EventTrigger.Entry();
                pointerEvent.eventID = eventTriggerType;
                pointerEvent.callback.AddListener((x) => onTriggerAction(x));
                eventrTrigger.triggers.Add(pointerEvent);
            }

            return eventrTrigger;
        }

        public static void RemoveAllTriggerEvents(Selectable button)
        {
            var triggers = button.gameObject.GetComponents<EventTrigger>();
            if (triggers.Length == 0)
                return;

            foreach (var trigger in triggers)
                trigger.Destroy();
        }

        public static Vector3 LinearInterp(
            Vector3 current,
            Vector3 target,
            float d)
        {
            var dX = target.x - current.x;
            var dY = target.y - current.y;
            var dZ = target.z - current.z;

            return new Vector3(current.x + dX * d, current.y + dY * d, current.z + dZ * d);
        }

        public static void DestroyChildren(this GameObject go, int startFrom = 0) => go.transform.DestroyChildren(startFrom);

        public static void DestroyChildren(this Transform transform, int startFrom = 0)
        {
            int counter = 0;
            foreach (Transform child in transform)
            {
                if (counter++ < startFrom)
                    continue;

                child.gameObject.Destroy();
            }
        }

        public static void DestroyAll<T>(this IEnumerable<T> list, bool allowDestroyAssets = false) where T : Object
        {
            foreach (var o in list)
                o.Destroy(allowDestroyAssets);
        }

        public static void DestroyAll<T>(this T[] list, bool allowDestroyAssets = false) where T : Object
        {
            foreach (var o in list)
                o.Destroy(allowDestroyAssets);
        }

        public static Vector2 ConvertWorldPosToCanvas(Vector3 pos, Camera camera, Vector2 canvasRectSizeDelta)
        {
            if (!IsInView(pos, camera))
                return canvasRectSizeDelta * 2;

            var viewportPosition = camera.WorldToViewportPoint(pos);
            var rawPos = new Vector2(
                viewportPosition.x * canvasRectSizeDelta.x - canvasRectSizeDelta.x * 0.5f,
                viewportPosition.y * canvasRectSizeDelta.y - canvasRectSizeDelta.y * 0.5f);
            return rawPos;
        }

        public static bool IsInView(Vector3 pos, Camera camera)
        {
            var pointOnScreen = camera.WorldToScreenPoint(pos);

            //Is in front
            if (pointOnScreen.z < 0)
            {
                return false;
            }

            //Is in FOV
            if ((pointOnScreen.x < 0) || (pointOnScreen.x > Screen.width) ||
                (pointOnScreen.y < 0) || (pointOnScreen.y > Screen.height))
            {
                return false;
            }

            return true;
        }

        public static bool Overlaps(this RectTransform a, RectTransform b)
        {
            return a.WorldRect().Overlaps(b.WorldRect());
        }

        public static bool Overlaps(this RectTransform a, RectTransform b, bool allowInverse)
        {
            return a.WorldRect().Overlaps(b.WorldRect(), allowInverse);
        }

        public static Rect WorldRect(this RectTransform rectTransform)
        {
            var r = rectTransform.rect;
            var s = rectTransform.lossyScale;
            float rectTransformWidth = r.width * s.x;
            float rectTransformHeight = r.height * s.y;

            Vector3 position = rectTransform.position;
            return new Rect(position.x - rectTransformWidth / 2f, position.y - rectTransformHeight / 2f, rectTransformWidth, rectTransformHeight);
        }

        public static void SetToDefault(this Transform tr)
        {
            tr.localPosition = Vector3.zero;
            tr.localRotation = Quaternion.identity;
            tr.localScale = Vector3.one;
        }

        public static void IterateOnMeAndChildren<T>(this Component o, Action<T> action) where T : Component
            => IterateOnMeAndChildren(o.gameObject, action);

        public static void IterateOnMeAndChildren<T>(this GameObject o, Action<T> applyAct) where T : Component
        {
            foreach (var child in o.GetComponentsInChildren<T>())
                applyAct.Invoke(child);
        }

        public static T[] GetAllComponents<T>(this GameObject o) where T : Component
        {
            var list = new List<T>();

            foreach (var child in o.GetComponentsInChildren<T>())
                list.Add(child);

            return list.ToArray();
        }

        public static void StretchAcrossParent(this RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
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
        
        public static void IterateOverMeAndChildren(GameObject obj, Action<GameObject> iterationAct, Func<GameObject, bool> needToGoDeepFunc)
        {
            iterationAct.Invoke(obj);

            if (!needToGoDeepFunc.Invoke(obj))
                return;

            foreach (Transform child in obj.transform)
                IterateOverMeAndChildren(child.gameObject, iterationAct, needToGoDeepFunc);
        }
        
        public static string ConvertToAssetPath(string fullPath)
        {
            string projectPath = Path.GetDirectoryName(Application.dataPath);

            // ReSharper disable once AssignNullToNotNullAttribute
            if (!fullPath.StartsWith(projectPath))
            {
                log.Error("The provided path is not part of the Unity project.");
                return null;
            }

            string relativePath = fullPath.Substring(projectPath.Length + 1).Replace('\\', '/');
            return relativePath;
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
        
        public static string Bold(this string text, bool isNeed = true) => !isNeed ? text : $"<b>{text}</b>";

        public static string ColorMark(this string text, bool isNeed = true, string color = "orange") => !isNeed ? text : $"<color={color}>{text}</color>";

        public static T RegisterForTransform<T>(Transform parent, Action<T> onInit = null, string optionalName = "") where T : Component
        {
            var comp = RegisterForScene(onInit, optionalName);
            comp.transform.SetParent(parent, false);
            return comp;
        }

        public static T RegisterForScene<T>(Action<T> onInit = null, string optionalName = "") where T : Component
        {
            var haveName = !optionalName.IsNullOrEmpty();
            var go = new GameObject(haveName ? optionalName : typeof(T).Name);
            var comp = go.AddComponent<T>();
            onInit?.Invoke(comp);
            return comp;
        }

        public static void Debug(this Bounds bounds, string name = "")
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name.IsNullOrEmpty() ? "DEBUG" : $"DEBUG_{name}";
            cube.transform.position = bounds.center;
            cube.transform.localScale = bounds.extents * 2;
        }
    }
}