using System.Linq;
using CycladeBase.Utils.Logging;
using UnityEditor;
using UnityEngine;

namespace CycladeBaseEditor.Editor
{
    public static class CycladeEditorHelpers
    {
        private static readonly Log log = new(nameof(CycladeEditorHelpers));

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