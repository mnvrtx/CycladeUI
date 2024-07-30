using System;
using System.Linq;

namespace Shared.Utils
{
    public class Enum<T> where T : Enum
    {
        public static T[] Values() => (T[])Enum.GetValues(typeof(T));

        public static T[] ValuesWithout(params T[] values) => ((T[])Enum.GetValues(typeof(T))).Where(q => !values.Contains(q)).ToArray();
    }
}