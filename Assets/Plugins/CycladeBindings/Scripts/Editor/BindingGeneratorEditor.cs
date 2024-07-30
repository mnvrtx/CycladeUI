using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CycladeBase.Utils;
using Shared.Utils.Logging;
using CycladeBaseEditor;
using CycladeBindings;
using CycladeBindings.Models;
using CycladeBindings.ScriptableObjects;
using CycladeBindings.UIStateSystem.Base;
using Shared.Utils;
using Solonity.View.Utils;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CycladeBindingsEditor
{
    [CustomEditor(typeof(BindingGenerator))]
    public class BindingGeneratorEditor : Editor
    {
        private static readonly Log log = new(nameof(BindingGeneratorEditor), CycladeDebugInfo.I);

        [NonSerialized] private Type _bindingType;
        [NonSerialized] private GlobalCycladeBindingsSettings _globalSettings;

        [SerializeField] private bool showInstruction;
        [SerializeField] private bool showDebug;
        [SerializeField] private bool showAdditional;

        private static readonly BindingInfo[] _defaultTypes =
        {
#if CYCLADEUI_TEXT_MESH_PRO
            new(typeof(TextMeshProUGUI), "Txt", true),
#endif
            new(typeof(Text), "Txt"),
            new(typeof(Image), "Img"),
            new(typeof(Button), "Btn"),
        };

        private string _helpText;
        [NonSerialized] private BindingGenerator _targetCache;

        private readonly CycladeEditorCommon _editorCommon = new();

        public override void OnInspectorGUI()
        {
            var settings = (BindingGenerator)target;

            if (_targetCache != settings)
            {
                _helpText = $"Instructions:\n" +
                           $"1. You can use the <b>\"{settings.bindTitle}\"</b> prefix. For example, <b>\"{settings.bindTitle} Title\"</b> will result in: <b>\"public Text TitleTxt;\"</b>\n" +
                           $"2. You can use the <b>\"{settings.bindArrTitle}\"</b> prefix. For example, <b>\"{settings.bindArrTitle} Title\"</b> will result in: <b>\"public List<Text> TitleTxtList;\"</b>\n" +
                           $"3. You can use the <b>\"{settings.bindViewInstanceTitle}\"</b> prefix. For example, <b>\"{settings.bindViewInstanceTitle} Title\"</b> will result in: <b>\"public ViewInstances<Text> TitleTxtInstances;\"</b>\n" +
                           $"4. You can specify the required component name following the colon \":\". For example, <b>\"{settings.bindTitle} Title:Text\"</b>.\n" +
                           $"If you don't, the system will use one or many of the following types: <b>{string.Join(", ", _defaultTypes.Select(q => q.Type.Name))}, Binding, GameObject</b>\n" +
                           $"5. You can specify short names. For example: {string.Join(", ", _defaultTypes.Where(q => !string.IsNullOrEmpty(q.OptionalBindingShortName)).Select(q => q.OptionalBindingShortName).Distinct())}\n" +
                           $"6. You can also specify a custom component name following the colon \":\". For example: [Bind] TurnRight:TouchingButton\n" +
                           $"7. You can also specify multiple component names following the \",\". For example: [Bind] TurnRight:RectTransform,TouchingButton";

                _targetCache = settings;
            }

            var currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (currentPrefabStage == null || currentPrefabStage.prefabContentsRoot != settings.gameObject)
            {
                EditorGUILayout.HelpBox($"Open the prefab for managing and adjusting its bindings.", MessageType.Info);
                return;
            }

            var loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(currentPrefabStage.assetPath);
            if (PrefabUtility.GetPrefabAssetType(loadedPrefab) == PrefabAssetType.Variant)
            {
                EditorGUILayout.HelpBox($"To adjust the bindings, please access and modify the original base prefab, as changes cannot be made directly to prefab variants.", MessageType.Info);
                return;
            }

            _globalSettings = CycladeEditorHelpers.TryFindGlobalSettings<GlobalCycladeBindingsSettings>();

            var fileAssetPath = GetFullPath(_globalSettings, settings, true);
            var fullPath = GetFullPath(_globalSettings, settings, false);

            var haveFile = File.Exists(fullPath);
            EditorGUILayout.LabelField($"PathToFile: {fileAssetPath} ({(haveFile ? "file exists" : "file not exists")})", EditorStyles.whiteLargeLabel);

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

            bool deleteExistsBinding = false;
            if (!string.IsNullOrEmpty(settings.lastGeneratedPath) && fullPath != settings.lastGeneratedPath)
            {
                EditorGUILayout.HelpBox($"The path of the new file is different from the generated path. Note that when you click 'Generate script', this old binding will be deleted.", MessageType.Warning);
                deleteExistsBinding = true;
            }

            if (GUILayout.Button($"1. {(haveFile ? "Update" : "Generate")} script"))
                Generate(settings, _bindingType, _globalSettings, haveFile, deleteExistsBinding, true);

            EditorGUI.BeginDisabledGroup(!haveFile);
            if (GUILayout.Button("2. Update bindings"))
                UpdateBindings(settings, false);
            EditorGUI.EndDisabledGroup();

            DrawStatesEditor(settings);
            DrawInstruction(settings);
            DrawAdditional(settings);
            DrawDebug(fullPath, settings);
        }

        private bool _showStates;

        private void DrawStatesEditor(BindingGenerator generator)
        {
            _showStates = EditorGUILayout.Foldout(_showStates, "States");

            if (!_showStates)
                return;

            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < generator.stateGroups.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                string newName = EditorGUILayout.TextField("State Group Name", generator.stateGroups[i].name, _editorCommon.BoldTextField);
                if (newName != generator.stateGroups[i].name)
                {
                    Undo.RecordObject(generator, "Edit State Group Name");
                    generator.stateGroups[i].name = newName;
                }

                if (GUILayout.Button("Remove"))
                {
                    Undo.RecordObject(generator, "Remove State Group");
                    generator.stateGroups.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                    break;
                }

                EditorGUILayout.EndHorizontal();

                // Edit states within the group
                for (int j = 0; j < generator.stateGroups[i].states.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    string newState = EditorGUILayout.TextField("State", generator.stateGroups[i].states[j]);
                    if (newState != generator.stateGroups[i].states[j])
                    {
                        Undo.RecordObject(generator, "Edit State");
                        generator.stateGroups[i].states[j] = newState;
                    }

                    if (GUILayout.Button("Remove State", GUILayout.Width(100)))
                    {
                        Undo.RecordObject(generator, "Remove State");
                        generator.stateGroups[i].states.RemoveAt(j);
                        EditorGUILayout.EndHorizontal();
                        break;
                    }

                    if (GUILayout.Button("Add below", GUILayout.Width(100)))
                    {
                        Undo.RecordObject(generator, "Add below");
                        generator.stateGroups[i].states.Insert(j + 1, $"State{generator.stateGroups[i].states.Count}");
                        EditorGUILayout.EndHorizontal();
                        break;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Add State"))
                {
                    Undo.RecordObject(generator, "Add State");
                    generator.stateGroups[i].states.Add($"State{generator.stateGroups[i].states.Count}");
                }

                _editorCommon.DrawUILine();
            }

            if (GUILayout.Button("Add State Group"))
            {
                Undo.RecordObject(generator, "Add State Group");
                generator.stateGroups.Add(new StatesGroup($"StateGroup{generator.stateGroups.Count}"));
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(generator);
            }
        }

        private void DrawAdditional(BindingGenerator settings)
        {
            showAdditional = EditorGUILayout.Foldout(showAdditional, "Additional");
            if (showAdditional)
            {
                EditorGUILayout.TextArea("Handlers will not be created for buttons that contain this part of the name", _editorCommon.RichHelp2);
                GUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"IgnoreButtonHandlerTypeName");

                settings.ignoreButtonHandlerType = _editorCommon.Property(() => EditorGUILayout.TextField(settings.ignoreButtonHandlerType),
                    this, $"Changed ignoreButtonHandlerName");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("HaveRootRectTransform");
                settings.haveRootRectTransform = _editorCommon.Property(() => EditorGUILayout.Toggle(settings.haveRootRectTransform),
                    settings, $"Changed HaveRootRectTransform");
                
                GUILayout.EndHorizontal();

                // Editor for string[] rootAdditionalComponents
                SerializedProperty rootAdditionalComponents = serializedObject.FindProperty("rootAdditionalComponents");
                
                EditorGUILayout.PropertyField(rootAdditionalComponents, new GUIContent("RootAdditionalComponents"), true);
                
                serializedObject.ApplyModifiedProperties();
                
                
                // GUILayout.EndHorizontal();
            }
        }

        private void DrawDebug(string fullPath, BindingGenerator settings)
        {
            showDebug = EditorGUILayout.Foldout(showDebug, "Debug");
            if (showDebug)
            {
                EditorGUILayout.LabelField($"PathToFileFull: {fullPath}");

                if (GUILayout.Button("TryToFindAndAddComponent"))
                    TryToFindAndProcessComponent(settings, true);

                if (GUILayout.Button("TryToRemoveComponent"))
                    TryToFindAndProcessComponent(settings, false);

                if (GUILayout.Button("ClearBinding"))
                    UpdateBindings(settings, true);
            }
        }

        private void DrawInstruction(BindingGenerator settings)
        {
            showInstruction = EditorGUILayout.Foldout(showInstruction, "Instruction");

            if (showInstruction)
            {
                EditorGUILayout.TextArea(_helpText, _editorCommon.RichHelp2);

                if (_globalSettings == null)
                    EditorGUILayout.HelpBox($"Not found {nameof(GlobalCycladeBindingsSettings)}. " +
                                            $"Please add this scriptable object to the project " +
                                            $"if you want to use different path instead of {GlobalCycladeBindingsSettings.DefaultPath}", MessageType.Info);
                else
                    EditorGUILayout.HelpBox($"{nameof(GlobalCycladeBindingsSettings)} scriptable object found", MessageType.Info);
            }
        }

        private void UpdateBindings(BindingGenerator settings, bool isClearMode)
        {
            if (!TryToFindAndProcessComponent(settings, true, false))
                return;

            SetBindings(settings, isClearMode);
        }

        private void SetBindings(BindingGenerator settings, bool isClearMode)
        {
            var namesHashSet = new HashSet<string>();

            ViewUtils.IterateOverMeAndChildren(settings.gameObject, go =>
            {
                if (settings.haveRootRectTransform && go == settings.gameObject)
                    ProcessBind(settings, _bindingType, go, null, namesHashSet, BindMode.Simple, false, isClearMode, GetRootRectTransformName());
                
                if (settings.rootAdditionalComponents.Length > 0 && go == settings.gameObject)
                    ProcessBind(settings, _bindingType, go, null, namesHashSet, BindMode.Simple, false, isClearMode, GetRootAdditionalComponentsName(settings));

                if (go.name.StartsWith(settings.bindTitle))
                    ProcessBind(settings, _bindingType, go, null, namesHashSet, BindMode.Simple, false, isClearMode);

                if (go.name.StartsWith(settings.bindArrTitle))
                    ProcessBind(settings, _bindingType, go, null, namesHashSet, BindMode.Array, false, isClearMode);

                if (go.name.StartsWith(settings.bindViewInstanceTitle))
                    ProcessBind(settings, _bindingType, go, null, namesHashSet, BindMode.ViewInstance, false, isClearMode);
            }, NeedToGoDeepFunc);

            BindStatefulElements(settings, isClearMode);

            log.Info("Bindings updated");
        }

        private static bool NeedToGoDeepFunc(GameObject o) => !PrefabUtility.IsAnyPrefabInstanceRoot(o) || o.GetComponent<BindingGenerator>() == null;

        public static void Generate(BindingGenerator settings, Type bindingType, GlobalCycladeBindingsSettings globalSettings, bool isUpdate, bool deleteExistsBinding, bool needToSave)
        {
            if (deleteExistsBinding)
            {
                var assetPath = ViewUtils.ConvertToAssetPath(settings.lastGeneratedPath);
                AssetDatabase.DeleteAsset(assetPath);
                DestroyAllBindings(settings);
            }

            var fields = new List<BindingFieldInfo>();

            var namesHashSet = new HashSet<string>();

            ViewUtils.IterateOverMeAndChildren(settings.gameObject, go =>
            {
                if (settings.haveRootRectTransform && go == settings.gameObject)
                    ProcessBind(settings, bindingType, go, fields, namesHashSet, BindMode.Simple, true, false, GetRootRectTransformName());
                
                if (settings.rootAdditionalComponents.Length > 0 && go == settings.gameObject)
                    ProcessBind(settings, bindingType, go, fields, namesHashSet, BindMode.Simple, true, false, GetRootAdditionalComponentsName(settings));

                if (go.name.StartsWith(settings.bindTitle))
                    ProcessBind(settings, bindingType, go, fields, namesHashSet, BindMode.Simple, true, false);

                if (go.name.StartsWith(settings.bindArrTitle))
                    ProcessBind(settings, bindingType, go, fields, namesHashSet, BindMode.Array, true, false);

                if (go.name.StartsWith(settings.bindViewInstanceTitle))
                    ProcessBind(settings, bindingType, go, fields, namesHashSet, BindMode.ViewInstance, true, false);
            }, NeedToGoDeepFunc);

            var btnsMethodInfos = new List<ButtonMethodInfo>();

            foreach (var field in fields)
            {
                if (!field.IsButton || field.Type.Contains(settings.ignoreButtonHandlerType) || field.BindMode == BindMode.ViewInstance)
                    continue;

                btnsMethodInfos.Add(new ButtonMethodInfo(field.Name, field.BindMode == BindMode.Array));
            }

            var fullPath = GetFullPath(globalSettings, settings,false);
            if (!FileHelper.IsValidPath(fullPath, out var errorMessage))
            {
                log.Error(errorMessage);
                return;
            }

            var uiBindingGenerator = new UiBinding
            {
                ClassName = GetBindingName(settings),
                Fields = fields,
                StateGroups = settings.stateGroups,
                ButtonMethodInfos = btnsMethodInfos,
            };
            FileHelper.WriteTextUtf8(fullPath, uiBindingGenerator.TransformText());

            settings.lastGeneratedPath = fullPath;

            if (needToSave)
            {
                EditorUtility.SetDirty(settings);
                SavePrefab();    
            }

            AssetDatabase.Refresh();
            log.Info($"The script has been {(isUpdate ? "updated" : "generated")}. Path: {GetFullPath(globalSettings, settings, true)}");
        }

        private static string GetRootRectTransformName()
        {
            return "RootRectTransform:RectTransform";
        }

        private static string GetRootAdditionalComponentsName(BindingGenerator settings)
        {
            if (settings.rootAdditionalComponents.Length == 1)
            {
                var singleComponent = settings.rootAdditionalComponents[0];
                return $"Root{singleComponent}:{singleComponent}";
            }
            return $"Root:{string.Join(",", settings.rootAdditionalComponents)}";
        }

        private static void SavePrefab()
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            var prefabRoot = prefabStage.prefabContentsRoot;
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabStage.assetPath);
            PrefabStageUtility.GetCurrentPrefabStage().ClearDirtiness();
        }

        private static string GetFullPath(GlobalCycladeBindingsSettings globalSettings, BindingGenerator settings, bool skipStart)
        {
            var baseBindingsPath = globalSettings != null ? globalSettings.baseBindingsPath : GlobalCycladeBindingsSettings.DefaultPath;
            var dataPath = !skipStart ? Path.GetFullPath(Application.dataPath) : "";
            return Path.Combine(dataPath, baseBindingsPath, settings.additionalFolderPath, $"{GetBindingName(settings)}.cs");
        }

        private static string GetBindingName(BindingGenerator settings) => $"{settings.name}{settings.bindPostfix}";

        private static void ProcessBind(BindingGenerator settings, Type bindingType, GameObject go, List<BindingFieldInfo> fields, HashSet<string> namesHashSet, BindMode bindMode, bool isRegister, bool isClearMode, string customName = null)
        {
            string title = "";
            if (bindMode == BindMode.Simple)
                title = settings.bindTitle;
            else if (bindMode == BindMode.Array)
                title = settings.bindArrTitle;
            else if (bindMode == BindMode.ViewInstance)
                title = settings.bindViewInstanceTitle;

            string determinedName = (string.IsNullOrEmpty(customName) ? go.name : customName)
                .Replace(title, "")
                .Replace(" ", "");

            bool fullName = false;

            List<BindingInfo> bindingInfos;

            Component bindingComponent = null;
            var isSpecifiedBinding = determinedName.Contains(':');

            if (!isSpecifiedBinding)
                bindingComponent = ViewUtils.FindComponent(go, c => c.GetType().Namespace == BindingConstants.GeneratedNameSpace, out _);

            if (bindingComponent != null)
            {
                bindingInfos = new List<BindingInfo>()
                {
                    new(bindingComponent.GetType(), "Bind"),
                };

                fullName = true;
            }
            else
            {
                string[] specifiedTypes = Array.Empty<string>();
                string firstSpecifiedType = string.Empty;
                if (isSpecifiedBinding)
                {
                    var split = determinedName.Split(':');
                    determinedName = split[0];
                    specifiedTypes = split[1].Split(',');
                    firstSpecifiedType = specifiedTypes[0];
                }

                //only contains transform, we are dealing with a GameObject binding
                if ((go.GetComponents<Component>().Length == 1 && go.GetComponent<RectTransform>() == null) || (isSpecifiedBinding && firstSpecifiedType == "GameObject" && specifiedTypes.Length == 1))
                {
                    bindingInfos = new List<BindingInfo>()
                    {
                        new(typeof(GameObject), "Go"),
                    };
                }
                else
                {
                    bindingInfos = _defaultTypes.ToList();

                    if (isSpecifiedBinding)
                    {
                        for (var i = 0; i < specifiedTypes.Length; i++)
                        {
                            var specifiedType = specifiedTypes[i];

                            //verify whether the default types include 'onlyInclude'.
                            if (bindingInfos.Any(q => q.Type.Name == specifiedType || q.OptionalBindingShortName == specifiedType))
                            {
                                bindingInfos.RemoveAll(q => q.Type.Name != specifiedType && q.OptionalBindingShortName != specifiedType);
                            }
                            else //if not, we are dealing with a non-default type for binding
                            {
                                if (i == 0)
                                    bindingInfos.Clear();

                                if (specifiedType == "GameObject")
                                {
                                    bindingInfos.Add(new BindingInfo(typeof(GameObject), "Go"));
                                }
                                else
                                {
                                    fullName = true;
                                    var foundComponent = FindComponent(go, specifiedType, out var foundComponents);
                                    if (foundComponent != null)
                                    {
                                        var type = foundComponent.GetType();
                                        if (string.IsNullOrEmpty(type.Namespace))
                                        {
                                            log.Error($"namespace in {type} is null. skip");
                                            continue;
                                        }

                                        bindingInfos.Add(new BindingInfo(type, specifiedTypes.Length == 1 ? "" : $"{type.Name}"));
                                    }
                                    else
                                    {
                                        log.Warn($"Not found component with name {specifiedType}. Found components: {FormatFoundComponents(foundComponents)}. skip: {go.name}", go);
                                    }    
                                }
                                
                                
                            }
                        }
                    }

                    bindingInfos.RemoveAll(q => q.Type != typeof(GameObject) && !go.TryGetComponent(q.Type, out _));

                    if (bindingInfos.Count == 0)
                    {
                        log.Warn($"Empty bind {determinedName}. skip: {go.name}. prefabName: {settings.name}", go);
                        return;
                    }
                }
            }

            SerializedObject serializedObject = default;
            if (!isRegister)
            {
                var component = settings.GetComponent(bindingType);
                // log.Debug($"component: {component.GetType().Name}", true);
                serializedObject = new SerializedObject(component);
            }

            foreach (var bInfo in bindingInfos)
            {
                var finalName = $"{determinedName}{bInfo.OptionalBindingShortName}";
                if (bindMode == BindMode.Array)
                    finalName += "List";
                if (bindMode == BindMode.ViewInstance)
                    finalName += "Instances";

                var determinedType = fullName || bInfo.NeedToFullName ? bInfo.Type.FullName : bInfo.Type.Name;
                var isButton = typeof(Button).IsAssignableFrom(bInfo.Type);

                string finalType = "";
                if (bindMode == BindMode.Simple)
                    finalType = determinedType;
                else if (bindMode == BindMode.Array)
                    finalType = $"List<{determinedType}>";
                else if (bindMode == BindMode.ViewInstance)
                    finalType = $"ViewInstances<{determinedType}>";

                if (isRegister)
                    TryRegisterBindingField(go, fields, namesHashSet, finalName, finalType, bindMode, isButton);
                else
                    TryBindField(go, namesHashSet, serializedObject, finalName, bInfo.Type, bindMode, isClearMode);
            }

            if (!isRegister)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(serializedObject.targetObject);
                serializedObject.Dispose();
            }
        }

        public void BindStatefulElements(BindingGenerator settings, bool isClearMode)
        {
            var component = settings.GetComponent(_bindingType);
            var serializedObject = new SerializedObject(component);

            var statefulElementsProp = serializedObject.FindProperty("statefulElements");
            if (statefulElementsProp != null)
            {
                statefulElementsProp.ClearArray();

                if (!isClearMode)
                {
                    var list = new List<BaseStatefulElement>();
                    ViewUtils.IterateOverMeAndChildren(settings.gameObject, go =>
                    {
                        var components = go.GetComponents<BaseStatefulElement>();
                        foreach (var statefulElement in components)
                            list.Add(statefulElement);
                    }, NeedToGoDeepFunc);

                    list = list.OrderBy(q => q.order).ToList();

                    foreach (var element in list)
                    {
                        statefulElementsProp.arraySize++;
                        statefulElementsProp.GetArrayElementAtIndex(statefulElementsProp.arraySize - 1).objectReferenceValue = element;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(serializedObject.targetObject);
            serializedObject.Dispose();
        }

        private static void TryBindField(GameObject go, HashSet<string> namesHashSet, SerializedObject serializedObject, string finalName, Type type, BindMode bindMode, bool isClearMode)
        {
            var prop = serializedObject.FindProperty(finalName);
            if (prop == null)
            {
                log.Warn($"Property with name {finalName} not found");
                return;
            }

            Object foundValue = null;
            if (!isClearMode)
            {
                if (typeof(GameObject).IsAssignableFrom(type))
                {
                    foundValue = go;
                }
                else
                {
                    foundValue = FindComponentByType(go, type, out var foundComponents);
                    if (foundValue == null)
                        log.Warn($"Component with name {type.FullName} not found in {go.name}. Found components: {FormatFoundComponents(foundComponents)}", go);
                }
            }

            if (prop.isArray)
            {
                var isFirst = namesHashSet.Add(finalName);
                if (isFirst)
                    prop.ClearArray();

                prop.arraySize++;
                prop.GetArrayElementAtIndex(prop.arraySize - 1).objectReferenceValue = foundValue;
            }
            else
            {
                if (bindMode == BindMode.ViewInstance)
                {
                    var templateProp = prop.FindPropertyRelative("template");
                    templateProp.objectReferenceValue = foundValue;
                }
                else
                {
                    prop.objectReferenceValue = foundValue;
                }
            }
        }

        private static string FormatFoundComponents(Component[] foundComponents) => string.Join(", ", foundComponents.Select(q => q.GetType().Name));

        private static Component FindComponent(GameObject go, string determinedType, out Component[] foundComponents)
            => ViewUtils.FindComponent(go, c => c.GetType().Name == determinedType || c.GetType().FullName == determinedType, out foundComponents);

        private static Component FindComponentByType(GameObject go, Type type, out Component[] foundComponents)
            => ViewUtils.FindComponent(go, type.IsInstanceOfType, out foundComponents);

        private static void TryRegisterBindingField(GameObject go, List<BindingFieldInfo> fields, HashSet<string> namesHashSet, string determinedName, string determinedType, BindMode bindMode, bool isButton)
        {
            if (!namesHashSet.Add(determinedName))
            {
                if (bindMode != BindMode.Array)
                    log.Error($"We already have bind for {determinedName}. skip: {go.name}. Change name and re-generate.", go);
                return;
            }

            fields.Add(new BindingFieldInfo(determinedType, determinedName, bindMode, isButton));
        }

        private bool TryToFindAndProcessComponent(BindingGenerator settings, bool isAdd, bool warnIfAdded = true)
        {
            if (!TryFindType(settings, out _bindingType))
                return false;

            if (isAdd)
            {
                if (!settings.gameObject.TryGetComponent(_bindingType, out _))
                {
                    var component = Undo.AddComponent(settings.gameObject, _bindingType);

                    // while (CycladeHelpers.GetIndexOfComponent(component) != 1) 
                    //     UnityEditorInternal.ComponentUtility.MoveComponentUp(component);

                    log.Info($"{_bindingType.Name} component added");
                }
                else if (warnIfAdded)
                {
                    log.Warn($"{_bindingType.Name} component already added", settings.gameObject);
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
                    log.Warn($"{_bindingType.Name} component already removed", settings.gameObject);
                }
            }

            return true;
        }

        private static void DestroyAllBindings(Component settings)
        {
            var components = settings.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component.GetType().Namespace == BindingConstants.GeneratedNameSpace)
                    DestroyImmediate(component);
            }
        }

        public static bool TryFindType(BindingGenerator settings, out Type type)
        {
            var types = CodeHelpers.FindTypesWith(t => t.Name == GetBindingName(settings) && t.IsSubclassOf(typeof(MonoBehaviour)));
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