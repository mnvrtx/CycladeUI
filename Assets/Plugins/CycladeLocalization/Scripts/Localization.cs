using System;
using System.IO;
using System.Linq;
using CycladeBase.Utils.Logging;
using CycladeStorage;
using Newtonsoft.Json;
using UnityEngine;

namespace CycladeLocalization
{
    //Usage: Localization.Get(Area.HeroNames, heroId)
    public class Localization
    {
        private static readonly Log log = new(nameof(Localization));

        public static readonly Localization I = new();
        public const string ResultLocalizationJsonsPath = "Localizations";
        private const string NotFound = "$NF{0}$";

        public bool IsNotLoaded => _locJson == null;

        public Enum CurrentLanguage;

        private LocJson _locJson;
        private Type _areaEnumType;
        
        public void Setup()
        {
            var langType = EnumTypeHelper.FindLang();
            _areaEnumType = EnumTypeHelper.FindArea();

            var langSettings = LocalStorage.I.GetSection<LanguageSettings>();
            Enum langEnum;
            if (Enum.TryParse(langType, langSettings.Language, out var lang))
                langEnum = (Enum)lang;
            else
                langEnum = (Enum)Enum.GetValues(langType).GetValue(0);
            LoadLanguage(langEnum);

            log.Debug($"Loaded {CurrentLanguage} localization from local");
        }

        public void LoadLanguage(Enum language)
        {
            CurrentLanguage = language;

            var path = Path.Combine(ResultLocalizationJsonsPath, $"localization_{CurrentLanguage}");
            var file = Resources.Load<TextAsset>(path);
            if (file != null)
            {
                _locJson = JsonConvert.DeserializeObject<LocJson>(file.text);
            }
            else
            {
                log.Warn($"not found localization at path: {path}");
            }
            Resources.UnloadAsset(file);
        }

        public string[] GetKeys(Enum areaEnum)
        {
            var area = areaEnum.ToString();

            if (!_locJson.Areas.ContainsKey(area))
                return Array.Empty<string>();

            return _locJson.Areas[area].Keys.ToArray();
        }

        public string Get(string areaEnum, string key, bool errOnNotFound = true, bool keyOnNotFound = false)
        {
            if (_locJson == null)
            {
                throw new Exception($"Isn't loaded. Please call \"{nameof(Localization)}.{nameof(I)}.{nameof(Setup)}()\"");
            }

            return Get((Enum)Enum.Parse(_areaEnumType, areaEnum), key, errOnNotFound, keyOnNotFound);
        }

        public static string Get(Enum areaEnum, string key, bool errOnNotFound = true, bool keyOnNotFound = false)
        {
            return I.GetText(areaEnum, key, errOnNotFound, keyOnNotFound);
        }

        public string GetText(Enum areaEnum, string key, bool errOnNotFound = true, bool keyOnNotFound = false)
        {
            if (_locJson == null)
            {
                log.Error($"Isn't loaded. Please call \"{nameof(Localization)}.{nameof(I)}.{nameof(Setup)}()\"");
                return "";
            }

            var area = areaEnum.ToString();
            if (!_locJson.Areas.ContainsKey(area))
            {
                if (errOnNotFound)
                    log.Warn($"Not found text area: {area}");
                return keyOnNotFound ? key : string.Format(NotFound, $"{area}:{key}");
            }

            if (string.IsNullOrWhiteSpace(key) || !_locJson.Areas[area].ContainsKey(key))
            {
                if (errOnNotFound)
                    log.Warn($"Not found text in area {area} with key: {key}");
                return keyOnNotFound ? key : string.Format(NotFound, $"{area}:{key}");
            }

            return _locJson.Areas[area][key];
        }
    }
}