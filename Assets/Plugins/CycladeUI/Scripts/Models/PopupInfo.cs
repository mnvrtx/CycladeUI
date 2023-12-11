using System;
using System.Linq;
using System.Reflection;

namespace CycladeUI.Models
{
    [Serializable]
    public class PopupInfo
    {
        public string assemblyName;
        public string fullName;

        public PopupInfo(string assemblyName, string fullName)
        {
            this.assemblyName = assemblyName;
            this.fullName = fullName;
        }

        public Type TryFind() => TryFind(assemblyName, fullName);

        public static Type TryFind(string assemblyName, string fullName)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch (Exception)
            {
                // ignored
            }

            if (assembly == null)
                return default;

            var type = assembly.GetType(fullName);
            return type;
        }

        protected bool Equals(PopupInfo other) => assemblyName == other.assemblyName && fullName == other.fullName;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PopupInfo)obj);
        }

        public override int GetHashCode() => HashCode.Combine(assemblyName, fullName);


        public override string ToString() => ToString(assemblyName, fullName);

        public string ToShortString() => ToShortString(fullName);

        public string ToShortestString() => ToShortestString(fullName);

        public static string ToString(string assemblyName, string fullName)
        {
            var assemblyShort = assemblyName.Split(',')[0];
            var split = fullName.Split('.');
            var name = $"<b>{split[split.Length - 1]}</b>";
            var nameSpace = string.Join(".", split.Take(split.Length - 1).ToArray());
            return $"{name} (namespace: {nameSpace}; assembly: {assemblyShort})";
        }

        public static string ToShortString(string fullName)
        {
            var split = fullName.Split('.');
            var name = $"<b>{split[split.Length - 1]}</b>";
            var nameSpace = string.Join(".", split.Take(split.Length - 1).ToArray());
            return $"{name} ({nameSpace})";
        }

        public static string ToShortestString(string fullName)
        {
            var split = fullName.Split('.');
            var name = split[split.Length - 1];
            return name;
        }
    }
}