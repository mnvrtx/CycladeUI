using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CycladeBindings;
using CycladeBindings.ScriptableObjects;
using CycladeBindings.Utils;
using CycladeBindings.Utils.Logging;
using CycladeBindingsEditor.Editor.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CycladeBindingsEditor.Editor
{
    [CustomEditor(typeof(BindingGenerator))]
    public class BindingGeneratorEditor : UnityEditor.Editor
    {
        private static readonly Log log = new(nameof(BindingGeneratorEditor));

        [NonSerialized] private Type _bindingType;
        [NonSerialized] private GlobalCycladeBindingsSettings _globalSettings;

        [SerializeField] private bool _showAdditional;
        [SerializeField] private bool _showDebug;

        private readonly (Type, string)[] _defaultTypes =
        {
#if CYCLADEUI_TEXT_MESH_PRO
            (typeof(TMPro.TMP_Text), "Txt"),
#endif
            (typeof(Text), "Txt"),
            (typeof(Image), "Img"),
            (typeof(Button), "Btn"),
        };

        private readonly Cache<GUIStyle> _helpBoxStyle = new(() =>
        {
            var helpBoxStyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
            helpBoxStyle.richText = true;
            helpBoxStyle.fontSize = 10;
            return helpBoxStyle;
        });

        public override void OnInspectorGUI()
        {
            var settings = (BindingGenerator)target;

            EditorGUILayout.TextArea($"Instructions:\n" +
                                     $"1. You can use the <b>\"{settings.bindTitle}\"</b> prefix. For example, <b>\"{settings.bindTitle} Title\"</b> will result in: <b>\"public Text TitleTxt;\"</b>\n" +
                                     $"2. You can use the <b>\"{settings.bindArrTitle}\"</b> prefix. For example, <b>\"{settings.bindArrTitle} Title\"</b> will result in: <b>\"public List<Text> TitleTxtList;\"</b>\n" +
                                     $"3. You can specify the required component name following the colon \":\". For example, <b>\"{settings.bindTitle} Title:Text\"</b>.\n" +
                                     $"If you don't, the system will use one or many of the following types: <b>{string.Join(", ", _defaultTypes.Select(q => q.Item1.Name))}</b>\n" +
                                     $"4. You can also specify a custom component name following the colon \":\".", 
                _helpBoxStyle);


            _globalSettings = EditorHelpers.TryFindGlobalSettings<GlobalCycladeBindingsSettings>();

            var filePath = GetFullPath(settings, true);
            var haveFile = File.Exists(GetFullPath(settings, false));
            EditorGUILayout.LabelField($"PathToFile: {filePath} ({(haveFile ? "file exists" : "file not exists")})", EditorStyles.whiteLargeLabel);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Optional additional folder: ", GUILayout.Width(150));
            {
                var before = settings.additionalFolderPath;
                var after = EditorGUILayout.TextField(before);
                if (before != after)
                {
                    serializedObject.FindProperty("additionalFolderPath").stringValue = after;
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Binding postfix: ", GUILayout.Width(150));
            {
                var before = settings.bindPostfix;
                var after = EditorGUILayout.TextField(before);
                if (before != after)
                {
                    serializedObject.FindProperty("bindPostfix").stringValue = after;
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.HelpBox($"Don't forget to press 2 buttons in sequence, after changing Bindings in the hierarchy.", MessageType.Info);

            if (GUILayout.Button("1. Update script"))
                Generate(settings);

            if (GUILayout.Button("2. Update bindings"))
            {
                if (TryFindAndProcessComponent(settings, true, false))
                    SetBindings(settings);
            }

            _showAdditional = EditorGUILayout.Foldout(_showAdditional, "Additional");
            if (_showAdditional)
            {
                if (_globalSettings == null)
                    EditorGUILayout.HelpBox($"Not found {nameof(GlobalCycladeBindingsSettings)}. " +
                                            $"Please add this scriptable object to the project " +
                                            $"if you want to use different path instead of {GlobalCycladeBindingsSettings.DefaultPath}", MessageType.Info);
                else
                    EditorGUILayout.HelpBox($"{nameof(GlobalCycladeBindingsSettings)} scriptable object found", MessageType.Info);
            }

            _showDebug = EditorGUILayout.Foldout(_showDebug, "Debug");
            if (_showDebug)
            {
                var fullPath = GetFullPath(settings, false);
                EditorGUILayout.LabelField($"PathToFileFull: {fullPath}");

                if (GUILayout.Button("TryFindAndAddComponent"))
                    TryFindAndProcessComponent(settings, true);

                if (GUILayout.Button("TryRemoveComponent"))
                    TryFindAndProcessComponent(settings, false);
            }
        }

        private void SetBindings(BindingGenerator settings)
        {
            var namesHashSet = new HashSet<string>();

            settings.transform.IterateOnMeAndChildren<Transform>(t =>
            {
                if (t.name.StartsWith(settings.bindTitle))
                    ProcessBind(settings, t, null, namesHashSet, false, false);

                if (t.name.StartsWith(settings.bindArrTitle))
                    ProcessBind(settings, t, null, namesHashSet, true, false);
            });
            log.Info("Bindings updated");
        }

        private void Generate(BindingGenerator settings)
        {
            var fields = new List<BindingFieldInfo>();

            var namesHashSet = new HashSet<string>();

            settings.transform.IterateOnMeAndChildren<Transform>(t =>
            {
                if (t.name.StartsWith(settings.bindTitle))
                    ProcessBind(settings, t, fields, namesHashSet, false, true);

                if (t.name.StartsWith(settings.bindArrTitle))
                    ProcessBind(settings, t, fields, namesHashSet, true, true);
            });

            var generator = new UiBinding
            {
                ClassName = GetBindingName(settings),
                Fields = fields,
            };

            var fullPath = GetFullPath(settings, false);
            if (!FileHelper.IsValidPath(fullPath, out var errorMessage))
            {
                log.Error(errorMessage);
                return;
            }

            FileHelper.WriteTextUtf8(fullPath, generator.TransformText());
            AssetDatabase.Refresh();
            log.Info($"Script generated. path: {GetFullPath(settings, true)}");
        }

        private static string GetBindingName(BindingGenerator settings) => $"{settings.name}{settings.bindPostfix}";

        private string GetFullPath(BindingGenerator settings, bool skipStart)
        {
            var baseBindingsPath = _globalSettings != null ? _globalSettings.baseBindingsPath : GlobalCycladeBindingsSettings.DefaultPath;
            var dataPath = !skipStart ? Path.GetFullPath(Application.dataPath) : "";
            return Path.Combine(dataPath, baseBindingsPath, settings.additionalFolderPath, $"{GetBindingName(settings)}.cs");
        }

        private void ProcessBind(BindingGenerator settings, Transform transform, List<BindingFieldInfo> fields, HashSet<string> namesHashSet, bool isArr, bool isRegister)
        {
            string determinedName = transform.name
                .Replace(!isArr ? settings.bindTitle : settings.bindArrTitle, "")
                .Replace(" ", "");

            var types = _defaultTypes.ToList();

            bool fullName = false;

            if (determinedName.Contains(':'))
            {
                var split = determinedName.Split(':');
                determinedName = split[0];
                var onlyInclude = split[1];

                if (types.Any(q => q.Item1.Name == onlyInclude))
                {
                    types.RemoveAll(q => q.Item1.Name != onlyInclude);
                }
                else
                {
                    fullName = true;
                    types.Clear();
                    var foundComponent = FoundComponent(transform, onlyInclude);
                    if (foundComponent != null)
                    {
                        types.Add((foundComponent.GetType(), ""));
                    }
                }
            }

            types.RemoveAll(q => !transform.TryGetComponent(q.Item1, out _));

            if (types.Count == 0)
            {
                log.Warn($"Empty bind {determinedName}. skip: {transform.gameObject.GetFullPath()}");
                return;
            }

            SerializedObject serializedObject = default;
            if (!isRegister)
            {
                var component = settings.GetComponent(_bindingType);
                // log.Debug($"component: {component.GetType().Name}", true);
                serializedObject = new SerializedObject(component);
            }

            foreach (var tuple in types)
            {
                var finalName = $"{determinedName}{tuple.Item2}";
                if (isArr)
                    finalName += "List";
                var determinedType = fullName ? tuple.Item1.FullName : tuple.Item1.Name;
                var finalType = !isArr ? determinedType : $"List<{determinedType}>";
                if (isRegister)
                    TryRegisterBindingField(transform, fields, namesHashSet, finalName, finalType, isArr);
                else
                    TryBindField(transform, namesHashSet, serializedObject, finalName, determinedType);
            }

            if (!isRegister)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
        }

        private static void TryBindField(Transform transform, HashSet<string> namesHashSet, SerializedObject serializedObject, string finalName, string determinedType)
        {
            var prop = serializedObject.FindProperty(finalName);
            if (prop == null)
            {
                log.Warn($"property with name {finalName} not found");
                return;
            }

            var foundComponent = FoundComponent(transform, determinedType);

            if (foundComponent == null)
                log.Warn($"component with name {determinedType} not found in {transform.gameObject.GetFullPath()}");
            // else
            //     log.Debug($"foundComponent: {foundComponent.GetType().Name}", true);

            if (prop.isArray)
            {
                var isFirst = namesHashSet.Add(finalName);
                if (isFirst)
                    prop.ClearArray();

                prop.arraySize++;
                prop.GetArrayElementAtIndex(prop.arraySize - 1).objectReferenceValue = foundComponent;
            }
            else
            {
                prop.objectReferenceValue = foundComponent;
            }
        }

        private static Component FoundComponent(Transform transform, string determinedType)
        {
            var components = transform.GetComponents<Component>();
            Component foundComponent = default;
            foreach (var component in components)
            {
                if (component.GetType().Name == determinedType || component.GetType().FullName == determinedType)
                {
                    foundComponent = component;
                    break;
                }
            }

            return foundComponent;
        }

        private static void TryRegisterBindingField(Transform t, List<BindingFieldInfo> fields, HashSet<string> namesHashSet, string determinedName, string determinedType, bool isArr)
        {
            if (!namesHashSet.Add(determinedName))
            {
                if (!isArr)
                    log.Warn($"We already have bind for {determinedName}. skip: {t.gameObject.GetFullPath()}. Change name and re-generate.");
                return;
            }

            fields.Add(new BindingFieldInfo(determinedType, determinedName));
        }

        private bool TryFindAndProcessComponent(BindingGenerator settings, bool isAdd, bool warnIfAdded = true)
        {
            if (!TryFindType(settings, out _bindingType))
                return false;

            if (isAdd)
            {
                if (!settings.gameObject.TryGetComponent(_bindingType, out _))
                {
                    Undo.AddComponent(settings.gameObject, _bindingType);
                    log.Info($"{_bindingType.Name} component added");
                }
                else if (warnIfAdded)
                {
                    log.Warn($"{_bindingType.Name} component already added");
                }
            }
            else
            {
                if (settings.gameObject.TryGetComponent(_bindingType, out var component))
                {
                    Undo.DestroyObjectImmediate(component);
                    log.Info($"{_bindingType.Name} component removed");
                }
                else
                {
                    log.Warn($"{_bindingType.Name} component already removed");
                }
            }

            return true;
        }

        private static bool TryFindType(BindingGenerator settings, out Type type)
        {
            var types = EditorHelpers.FindTypesWith(t => t.Name == GetBindingName(settings) && t.IsSubclassOf(typeof(MonoBehaviour)));
            if (types.Count == 0)
            {
                log.Warn($"Not found {GetBindingName(settings)}");
                type = default;
                return false;
            }

            if (types.Count > 1)
                log.Error($"More than one {GetBindingName(settings)} in the project. Just picking the first one but you need to remove the second one.");

            type = types[0];
            return true;
        }
    }
}