using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CycladeBase.Utils;
using CycladeLocalization;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace CycladeLocalizationEditor
{
    public class GoogleSheetsSyncManager
    {
        public const string UpdateCommandFullName = "Cyclade/Localization/Update \"localization.json\" files from Google Sheet";

        private const string LogTag = "[LocalizationTool]";
        private const string NotFoundLocText = "$NO_LOC_IN_GSHEET$";
        private const string NotUsingKey = "NOT_USING";
        private const string CredentialsFileName = "google_sheets_credential.json";
        private const string InfoFileName = "sheet_info.json";

        [MenuItem(UpdateCommandFullName)]
        public static void SynchronizeLocalization()
        {
            var langType = EnumTypeHelper.FindLang();
            var areaType = EnumTypeHelper.FindArea();

            var scopes = new []
            {
                SheetsService.Scope.SpreadsheetsReadonly,
            };

            string json = File.ReadAllText(Path.Combine(Application.dataPath, "Editor", "CycladeSettings", CredentialsFileName));
            GoogleCredential credential = GoogleCredential.FromJson(json).CreateScoped(scopes);

            var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential, 
                ApplicationName = "Localization Tool Editor",
            });

            var locJsons = new Dictionary<string, LocJson>();
            {
                var languages = Enum.GetValues(langType).Cast<Enum>().ToArray();

                foreach (var language in languages) {
                    locJsons.Add(language.ToString(), new LocJson
                    {
                        Areas = new Dictionary<string, Dictionary<string, string>>()
                    });
                }
            }
            
            string infoJson = File.ReadAllText(Path.Combine(Application.dataPath, "Editor", "CycladeSettings", InfoFileName));
            var rawInfo = JObject.Parse(infoJson);
            var sheetId = rawInfo.Property("sheet_id").Value.ToString();

            var nfLocVal = new Dictionary<string, int>();
            int notUsingKeysCount = 0;

            var areas = Enum.GetValues(areaType).Cast<Enum>().ToArray();

            foreach (var area in areas) {
                var entryArea = area.ToString();
                var sheetName = entryArea.ToLower();
                var request = service.Spreadsheets.Values.Get(sheetId, sheetName);
                IList<IList<object>> rows = request.Execute().Values;

                bool isTitle = true;
                var languageIndexes = new List<Optional<string>>();

                for (var j = 0; j < rows.Count; j++) {
                    IList<object> row = rows[j];

                    if (isTitle) {
                        for (int i = 1; i < row.Count; i++) {
                            var langStr = row[i].ToString().Replace('-','_');

                            if (Enum.TryParse(langType, langStr, true, out object lang))
                                languageIndexes.Add(new Optional<string>(lang.ToString()));
                            else
                                languageIndexes.Add(new Optional<string>(null));
                        }
                        isTitle = false;
                        continue;
                    }

                    if (row.Count == 0) {
                        Debug.LogWarning($"{LogTag}: Row is empty. SheetName: {sheetName}");
                        continue;
                    }

                    var keyStr = row[0].ToString();

                    if (string.IsNullOrEmpty(keyStr)) {
                        Debug.LogWarning($"{LogTag}: Key IsNullOrWhitespace. SheetName: {sheetName}");
                        continue;
                    }

                    if (keyStr.Contains(NotUsingKey)) {
                        notUsingKeysCount++;
                        continue;
                    }

                    for (int i = 0; i < languageIndexes.Count; i++) {
                        var valueIdx = i + 1;
                        var locFound = valueIdx < row.Count;
                        var valueStr = locFound ? row[valueIdx].ToString().Replace("\\n", "\n") : NotFoundLocText;
                        var langValue = languageIndexes[i];
                        
                        if (!langValue.HasValue)
                            continue;

                        var lang = langValue.Value;

                        if (!locFound)
                            nfLocVal[lang] = nfLocVal.GetValueOrDefault(lang) + 1;

                        var locJson = locJsons[lang];

                        if (!locJson.Areas.ContainsKey(entryArea))
                            locJson.Areas.Add(entryArea, new Dictionary<string, string>());

                        if (locJson.Areas[entryArea].ContainsKey(keyStr)) {
                            var tempKey = $"temporaryInvalidKey{j}";
                            Debug.LogWarning($"{LogTag}: Duplicate key: {keyStr} in {sheetName}. Set key to {tempKey}");
                            keyStr = tempKey;
                        }
                        locJson.Areas[entryArea][keyStr] = valueStr;
                    }
                }
            }

            foreach (var locJson in locJsons) {
                var path = Path.Combine(Application.dataPath, "Resources", Localization.ResultLocalizationJsonsPath, $"localization_{locJson.Key}.json");
                FileHelper.WriteTextUtf8(path, JsonConvert.SerializeObject(locJson.Value, Formatting.Indented));
                Debug.Log($"{LogTag} Synchronization {locJson.Key} to {path}");
            }
            
            AssetDatabase.Refresh();

            Debug.Log($"{LogTag} Synchronization was completed!");
            if (nfLocVal.Count > 0)
                Debug.LogWarning($"{LogTag} Not found localization values list: [{string.Join(", ", nfLocVal.Select(q => $"{q.Key}: {q.Value}"))}]. Search by \"{NotFoundLocText}\" in files");
            if (notUsingKeysCount > 0)
                Debug.LogWarning($"{LogTag} {notUsingKeysCount} localization values is not using. Search by \"{NotUsingKey}\" in files");
        }
    }
}