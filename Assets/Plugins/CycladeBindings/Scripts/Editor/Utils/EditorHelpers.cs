using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CycladeBindings.Utils.Logging;
using UnityEditor;
using UnityEngine;

namespace CycladeBindingsEditor.Editor.Utils
{
    public static class EditorHelpers
    {
        private static readonly Log log = new(nameof(EditorHelpers));

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

        public static T TryFindGlobalSettings<T>() where T : ScriptableObject
        {
            var settingsArray = FindScriptableObjects<T>();
            if (settingsArray.Length == 0)
                return default;

            if (settingsArray.Length > 1)
            {
                log.Error($"Please ensure that only one '{typeof(T).Name}' scriptable object is present in the project.");
                return default;
            }

            return settingsArray.Single();
        }
        
        public static T[] FindScriptableObjects<T>() where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            var assets = new T[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }

            return assets;
        }
    }
}