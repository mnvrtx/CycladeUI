using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Shared.Utils.Logging;

namespace Shared.Utils
{
    public static class CodeHelpers
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rnd)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var array = source.ToArray();
            var n = array.Length;
            for (var i = 0; i < n; i++)
            {
                // Exchange a[i] with random element in a[i..n-1]
                var r = i + rnd.Next(0, n - i);
                // ReSharper disable once SwapViaDeconstruction
                var temp = array[i];
                array[i] = array[r];
                array[r] = temp;
            }

            return array;
        }

        public static T RandomItem<T>(this ICollection<T> list, Func<T, float> getWeight, Random rnd)
        {
            var totalWeight = 0f;
            foreach (var item in list)
            {
                var weight = getWeight(item);
                if (weight > 0)
                {
                    totalWeight += weight;
                }
            }

            var randomWeight = (float)(rnd.NextDouble() * totalWeight);

            return PickRandomItemByWeight(list, getWeight, randomWeight);
        }

        private static T PickRandomItemByWeight<T>(IEnumerable<T> list, Func<T, float> getWeight, float randomWeight)
        {
            var currentWeight = 0f;

            foreach (var item in list)
            {
                var weight = getWeight(item);
                if (weight > 0)
                {
                    currentWeight += weight;
                    if (randomWeight <= currentWeight)
                    {
                        return item;
                    }
                }
            }

            return default;
        }

        public static string ShortName(this Type t)
        {
            if (!t.IsGenericType)
                return t.Name;

            string genericName = t.GetGenericTypeDefinition().Name;
            genericName = genericName.Substring(0, genericName.IndexOf('`'));

            string genericArgs = string.Join(",", t.GetGenericArguments().Select(ShortName).ToArray());
            return $"{genericName}<{genericArgs}>";
        }

        public static void AddToInnerList<TKey, TListType>(this Dictionary<TKey, List<TListType>> dict, TKey key, TListType value)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, new List<TListType>());
            var list = dict[key];
            list.Add(value);
        }

        public static void SetValue<TKey1, TKey2, TVal>(this Dictionary<TKey1, Dictionary<TKey2, TVal>> dict, TKey1 key1, TKey2 key2, TVal val)
        {
            Dictionary<TKey2, TVal> innerDict;
            if (!dict.TryGetValue(key1, out innerDict))
            {
                innerDict = new Dictionary<TKey2, TVal>();
                dict[key1] = innerDict;
            }

            innerDict[key2] = val;
        }

        public static TVal GetOrDefault<TVal, TKey>(this Dictionary<TKey, TVal> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var val))
                return val;
            return default;
        }

        public static T ModifyObject<T>(this T obj, Action<T> modify)
        {
            modify.Invoke(obj);
            return obj;
        }

        public static T GetRandom<T>(this IList<T> list) => GetRandom(list, new Random());

        public static T GetRandom<T>(this IList<T> list, Random rnd)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (list.Count == 0)
                throw new InvalidOperationException($"{nameof(list)} is empty");
            return list[rnd.Next(0, list.Count)];
        }

        /// <summary>
        /// Метод считает полную цену исходя из скидочной и размера самой скидки
        /// </summary>
        /// <param name="price">Скидочная цена. Например 90</param>
        /// <param name="discount">Размер скидки. Например 10</param>
        /// <returns>Полная цена</returns>
        public static float CalculateFullPrice(float price, int discount)
        {
            return price / (100 - discount) * 100;
        }

        public static string FormatPrice(decimal price, string currencyCode)
        {
            var final = $"{currencyCode} {price:N2}";
            return final.Replace(',', '.');
        }

        public static IEnumerable<IEnumerable<T>> SplitByChunks<T>(this IEnumerable<T> source, int len)
        {
            if (len == 0)
                throw new ArgumentNullException();

            var enumer = source.GetEnumerator();
            while (enumer.MoveNext())
            {
                yield return Take(enumer.Current, enumer, len);
            }
        }

        private static IEnumerable<T> Take<T>(T head, IEnumerator<T> tail, int len)
        {
            while (true)
            {
                yield return head;
                if (--len == 0)
                    break;
                if (tail.MoveNext())
                    head = tail.Current;
                else
                    break;
            }
        }

        public static string Trim(this long frameIdx, int count = 3)
        {
            var str = frameIdx.ToString();
            if (str.Length < count)
                return str;
            return str.Substring(str.Length - count);
        }

        public static string TimeStr(TimeSpan time)
        {
            return $"{time.Hours,2:00}:{time.Minutes,2:00}:{time.Seconds,2:00}";
        }

        public static string FormatFirstNBytes(this byte[] b, int n)
        {
            return
                $"Buffer (first <={n} bytes) {b.Take(b.Length > n ? n : b.Length).ToArray().VisualizeBytesArray()}{(b.Length > n ? "..." : "")}";
        }

        public static string VisualizeBytesArray(this byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            for (int i = 0; i < ba.Length; i++)
            {
                hex.AppendFormat("{0:x2}", ba[i]);
                if (i != ba.Length - 1)
                    hex.Append("-");
            }

            return hex.ToString();
        }

        public static T Pop<T>(this List<T> list)
        {
            var lastEl = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return lastEl;
        }

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

        public static T RemoveAtAndGet<T>(this List<T> list, int i)
        {
            var ret = list[i];
            list.RemoveAt(i);
            return ret;
        }

        public static Regex GetDefaultValidationRegex() => new(@"^[a-zA-Z0-9_@!&|]{1,10}$");
        public static string GetDefaultValidationError() => "Input must be between 1 and 10 characters long and contain only alphanumeric characters, underscores, '@', '!', '&', and '|'.";
        
        public static int ValidateNickName(string nickName)
        {
            if (nickName.IsNullOrEmpty())
                return -1;

            var regexNickText = GetDefaultValidationRegex();
            if (!regexNickText.IsMatch(nickName))
                return -2;

            return 1;
        }

        // public static long GetNextWeekday(DayOfWeek day)
        // {
        //     var result = DateTime.Today;
        //
        //     if (result.DayOfWeek == day)
        //     {
        //         result = result.AddDays(7);
        //     }
        //     else
        //     {
        //         while (result.DayOfWeek != day)
        //         {
        //             result = result.AddDays(1);
        //         }
        //     }
        //
        //
        //     return ((DateTimeOffset)result).ToUnixTimeMilliseconds();
        // }

        public static string ToDebugJson<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return "{\n" + string.Join("\n", dictionary.Select(kv => $"\"{kv.Key}\"=({kv.Value})")) + "\n}";
        }

        public static Dictionary<TValue, TKey> SwapKeysAndValues<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return dictionary.ToDictionary(t => t.Value, t => t.Key);
        }

        public static T InvokePrivateOrOtherMethod<T>(this object obj, string name)
        {
            MethodInfo dynMethod = obj.GetType().GetMethod(name,
                BindingFlags.NonPublic | BindingFlags.Instance);
            var result = dynMethod.Invoke(obj, new object[] { });
            return (T)result;
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

        public static int SafeParseInt(string s, int defaultVal = -1)
        {
            if (string.IsNullOrEmpty(s))
                return defaultVal;

            int.TryParse(s, out defaultVal);
            return defaultVal;
        }

        public static string SToPrettyVal(this int val)
        {
            if (val >= 1000000)
                return (val / 1000000D).ToString("0.#") + "M";
            if (val >= 1000)
                return (val / 1000D).ToString("0.#") + "K";
            return val.ToString("N0");
        }

        public static string ToPluralText(this int count, string singular, string plural = null, bool capitalize = false)
        {
            if (count == 1)
            {
                return $"{count} {singular}";
            }
            else
            {
                // If no plural form is provided, try to form the plural
                if (plural == null)
                {
                    // Basic pluralization rule
                    if (singular.EndsWith("y") && !singular.EndsWith("ay") && !singular.EndsWith("ey") && !singular.EndsWith("iy") && !singular.EndsWith("oy") && !singular.EndsWith("uy"))
                    {
                        plural = singular.Substring(0, singular.Length - 1) + "ies";
                    }
                    else if (singular.EndsWith("s") || singular.EndsWith("sh") || singular.EndsWith("ch") || singular.EndsWith("x") || singular.EndsWith("z"))
                    {
                        plural = singular + "es";
                    }
                    else
                    {
                        plural = singular + "s";
                    }

                    if (capitalize)
                        plural = plural.ToUpper(CultureInfo.InvariantCulture);
                }

                return $"{count} {plural}";
            }
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

        public static string ArrToString<T>(this IEnumerable<T> obj) => string.Join(", ", obj);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TryGet<T>(this T[] arr, int idx) => arr != null && idx > -1 && idx < arr.Length ? arr[idx] : default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TryGet<T>(this IReadOnlyList<T> list, int idx) => list != null && idx > -1 && idx < list.Count ? list[idx] : default;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TryGetFromLast<T>(this T[] arr, int idx) => arr != null && idx > -1 && idx < arr.Length ? arr[arr.Length - 1 - idx] : default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TryGetFromLast<T>(this IReadOnlyList<T> list, int idx) => list != null && idx > -1 && idx < list.Count ? list[list.Count - 1 - idx] : default;

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

        public static bool IsIndexValid<T>(this IList<T> list, int idx) => list != null && idx > -1 && idx < list.Count;

        public static bool IsIndexValid<T>(this T[] arr, int idx) => idx > -1 && idx < arr.Length;
        
        public static string GetIndexError<T>(this T[] arr, int idx) => idx > -1 && idx < arr.Length ? "" : $"Index Error: The index must be: `0 <= index < {arr.Length}`, but it is {idx}";

        public static string GetIndexError<T>(this IList<T> list, int idx) => list != null && idx > -1 && idx < list.Count ? "" : $"Index Error: The index must be: `0 <= index < {list.Count}`, but it is {idx}";

        public static bool IsIndexValid<T>(this T[] arr, int idx, Log log)
        {
            if (arr.IsIndexValid(idx))
                return true;
            log.Warn(arr.GetIndexError(idx));
            return false;
        }
        
        public static bool IsIndexValid<T>(this IList<T> arr, int idx, Log log)
        {
            if (arr.IsIndexValid(idx))
                return true;
            log.Warn(arr.GetIndexError(idx));
            return false;
        }

        public static string ListToString<T>(this IEnumerable<T> list)
        {
            return string.Join(", ", list.Cast<object>().Select(q => q.GetType().Name));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static int FindIndex<TColl, TItem>(this TColl collection, TItem item)
            where TColl : IReadOnlyList<TItem>
            where TItem : class
        {
            for (int i = 0; i < collection.Count; ++i)
            {
                if (collection[i] == item)
                {
                    return i;
                }
            }

            return -1;
        }

        public static string OrOverride(this string defaultStr, string overridedStr)
            => !string.IsNullOrEmpty(overridedStr) ? overridedStr : defaultStr;

        public static string Truncate(this string value, int maxLength, string truncationSuffix = "…")
        {
            if (string.IsNullOrEmpty(value))
                return value;
            
            return value.Length > maxLength
                       ? value.Substring(0, maxLength) + truncationSuffix
                       : value;
        }

        public static T2 AddIfNotAndGet<T, T2>(this Dictionary<T, T2> dict, T key, Func<T2> valueCreator)
        {
            T2 result;
            if (!dict.ContainsKey(key))
            {
                result = valueCreator.Invoke();
                dict.Add(key, result);
            }
            else
            {
                result = dict[key];
            }

            return result;
        }

        public static string TrimMiddle(this string input, int maxCharacters = 15)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= maxCharacters)
            {
                return input;
            }

            var diff = (input.Length - maxCharacters) / 2;

            int mid = input.Length / 2;
            int start = mid - diff;
            int end = mid + diff;

            return input.Substring(0, start) + "..." + input.Substring(end);
        }
        
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");

            var arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf(arr, src) + 1;
            return (arr.Length == j) ? arr[0] : arr[j];            
        }

        public static string Repeat(this string str, int count)
        {
            var result = "";
            for (int i = 0; i < count; i++)
            {
                result += str;
            }

            return result;
        }
        
        public static bool SetToCurrentAndToMaxIfReached(this int value, ref int current, ref int max)
        {
            current = value;
            if (max < current)
            {
                max = current;
                return true;
            }

            return false;
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

        public static bool EnsureCapacity<T>(ref T[] array, int capacity, bool fillEmpty = false)
        {
            if (array == null)
            {
                array = new T[capacity];
                if (fillEmpty)
                {
                    for (var i = 0; i < capacity; i++)
                    {
                        array[i] = Activator.CreateInstance<T>();
                    }
                }

                return true;
            }

            var before = array.Length;
            var isNewArray = array.Length < capacity;
            if (isNewArray)
            {
                var newArray = new T[capacity];
                array.CopyTo(newArray, 0);
                array = newArray;
                if (fillEmpty)
                {
                    for (var i = before; i < capacity; i++)
                    {
                        array[i] = Activator.CreateInstance<T>();
                    }
                }
            }

            return isNewArray;
        }

        public static int ProjectToValueBetween(this float value, int min, int max)
        {
            return (int)Math.Round(min + value * (max - min));
        }
        
        public static float ProjectToPercentage(this int value, int min, int max)
        {
            if (max <= min)
                throw new ArgumentException("max must be greater than min");

            return (float)(value - min) / (max - min);
        }
    }
}