using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace CycladeUI.Utils
{
    public static class Helpers
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

        public static void SetParentImitate(this RectTransform rect)
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
    }
}