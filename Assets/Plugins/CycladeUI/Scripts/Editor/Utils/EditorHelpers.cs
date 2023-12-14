using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CycladeUIEditor.Utils
{
    public static class EditorHelpers
    {
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
    }
}