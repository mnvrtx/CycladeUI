using System;
using System.Collections.Generic;
using System.Linq;
using CycladeBase.Utils;
using CycladeBase.Utils.Logging;
using Newtonsoft.Json;
using UnityEngine;

namespace CycladeStorage
{
    public class LocalStorage
    {
        private static readonly Log log = new(nameof(LocalStorage));

        private static LocalStorage _instance;

        public static bool IsDebug;
        public static LocalStorage I => _instance ??= new LocalStorage();

        private readonly Dictionary<Type, IStorageSection> _sections = new();

        private readonly Dictionary<string, Type> KeyToType;

        private readonly Dictionary<Type, string> TypeToKey;

        private LocalStorage()
        {
            KeyToType = CycladeHelpers.FindTypesWith(t => typeof(IStorageSection).IsAssignableFrom(t) && t.IsClass)
                .ToDictionary(q2 => $"{q2.Name}Key", q => q);
            
            TypeToKey = KeyToType.SwapKeysAndValues();
            
            foreach (var keyTypeKvp in KeyToType)
            {
                (Type key, IStorageSection data) loadedSection = LoadSection(keyTypeKvp.Value);
                _sections.Add(loadedSection.key, loadedSection.data);
            }

            log.Debug($"Loaded from PlayerPrefs. Sections: {_sections.Count}", IsDebug); 
        }

        public T GetSection<T>() where T : IStorageSection => (T)_sections[typeof(T)];

        public T ModifySection<T>(Action<T> onChange)
            where T : IStorageSection
        {
            var type = typeof(T);
            var section = (T)_sections[type];

            onChange.Invoke(section);

            SaveSection(type);
            return section;
        }

        public void ResetAll()
        {
            log.Debug($"Started reset all", IsDebug);

            foreach (var section in _sections)
                PlayerPrefs.DeleteKey(TypeToKey[section.Key]);

            log.Debug($"Removed all keys({_sections.Count}) from storage", IsDebug);
        }

        public void SaveAll()
        {
            foreach (var section in _sections)
                SaveSection(section.Key, false);

            PlayerPrefs.Save();
            log.Debug($"Saved all {_sections.Count} sections", IsDebug);
        }

        private void SaveSection(Type type, bool needCommit = true)
        {
            var key = TypeToKey[type];
            var value = JsonConvert.SerializeObject(_sections[type]);
            PlayerPrefs.SetString(key, value);
            if (needCommit)
            {
                PlayerPrefs.Save();
                log.Debug($"Saved {key} section. Details: {value}", IsDebug);
            }
        }

        private (Type, IStorageSection) LoadSection(Type type)
        {
            var key = TypeToKey[type];
            var section = PlayerPrefs.HasKey(key) ? TryToLoadJsonFromPrefs(type, key) : Activator.CreateInstance(type);

            return (type, (IStorageSection)section);
        }

        private static object TryToLoadJsonFromPrefs(Type type, string key)
        {
            object loadedSection;

            var jsonRaw = PlayerPrefs.GetString(key);

            try
            {
                loadedSection = JsonConvert.DeserializeObject(jsonRaw, type);
            }
            catch (Exception e)
            {
                loadedSection = Activator.CreateInstance(type);
                PlayerPrefs.SetString(key, JsonConvert.SerializeObject(loadedSection));
                log.Warn($"Section {key}(jsonRaw: {jsonRaw}) deserialization error: {e.Message}.\nReset and saved successfully.");
            }

            return loadedSection;
        }

        public static void Reset()
        {
            _instance = null;
        }
    }
}