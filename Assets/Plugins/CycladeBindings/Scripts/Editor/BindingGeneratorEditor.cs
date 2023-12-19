using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CycladeBase.Utils;
using CycladeBase.Utils.Logging;
using CycladeBaseEditor.Editor;
using CycladeBindings;
using CycladeBindings.Models;
using CycladeBindings.ScriptableObjects;
using CycladeBindings.UIStateSystem.Base;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CycladeBindingsEditor.Editor
{
    [CustomEditor(typeof(BindingGenerator))]
    public class BindingGeneratorEditor : UnityEditor.Editor
    {
        private static readonly Log log = new(nameof(BindingGeneratorEditor));

        [NonSerialized] private Type _bindingType;
        [NonSerialized] private GlobalCycladeBindingsSettings _globalSettings;

        [SerializeField] private bool showInstruction;
        [SerializeField] private bool showDebug;

        private readonly BindingInfo[] _defaultTypes =
        {
#if CYCLADEUI_TEXT_MESH_PRO
            new(typeof(TextMeshProUGUI), "Txt", true),
#endif
            new(typeof(Text), "Txt"),
            new(typeof(Image), "Img"),
            new(typeof(Button), "Btn"),
        };

        private readonly CycladeEditorCommon _editorCommon = new();

        private bool _isClearMode;

        public override void OnInspectorGUI()
        {
            var settings = (BindingGenerator)target;

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

            var fileAssetPath = GetFullPath(settings, true);
            var fullPath = GetFullPath(settings, false);

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
                Generate(settings, haveFile, deleteExistsBinding);

            EditorGUI.BeginDisabledGroup(!haveFile);
            if (GUILayout.Button("2. Update bindings"))
                UpdateBindings(settings, false);
            EditorGUI.EndDisabledGroup();

            DrawStatesEditor(settings);
            DrawInstruction(settings);
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
                EditorGUILayout.TextArea($"Instructions:\n" +
                                         $"1. You can use the <b>\"{settings.bindTitle}\"</b> prefix. For example, <b>\"{settings.bindTitle} Title\"</b> will result in: <b>\"public Text TitleTxt;\"</b>\n" +
                                         $"2. You can use the <b>\"{settings.bindArrTitle}\"</b> prefix. For example, <b>\"{settings.bindArrTitle} Title\"</b> will result in: <b>\"public List<Text> TitleTxtList;\"</b>\n" +
                                         $"3. You can use the <b>\"{settings.bindViewInstanceTitle}\"</b> prefix. For example, <b>\"{settings.bindViewInstanceTitle} Title\"</b> will result in: <b>\"public List<Text> TitleTxtInstances;\"</b>\n" +
                                         $"4. You can specify the required component name following the colon \":\". For example, <b>\"{settings.bindTitle} Title:Text\"</b>.\n" +
                                         $"If you don't, the system will use one or many of the following types: <b>{string.Join(", ", _defaultTypes.Select(q => q.Type.Name))}, Binding, GameObject</b>\n" +
                                         $"5. You can also specify a custom component name following the colon \":\".",
                    _editorCommon.RichHelp2);

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

            _isClearMode = isClearMode;
            SetBindings(settings);
        }

        private void SetBindings(BindingGenerator settings)
        {
            var namesHashSet = new HashSet<string>();

            IterateOverMeAndChildren(settings.gameObject, go =>
            {
                if (go.name.StartsWith(settings.bindTitle))
                    ProcessBind(settings, go, null, namesHashSet, BindMode.Simple, false);

                if (go.name.StartsWith(settings.bindArrTitle))
                    ProcessBind(settings, go, null, namesHashSet, BindMode.Array, false);
                
                if (go.name.StartsWith(settings.bindViewInstanceTitle))
                    ProcessBind(settings, go, null, namesHashSet, BindMode.ViewInstance, false);
                
            }, o => !PrefabUtility.IsAnyPrefabInstanceRoot(o));

            BindStatefulElements(settings);

            log.Info("Bindings updated");
        }

        private void Generate(BindingGenerator settings, bool isUpdate, bool deleteExistsBinding)
        {
            if (deleteExistsBinding)
            {
                var assetPath = CycladeHelpers.ConvertToAssetPath(settings.lastGeneratedPath);
                AssetDatabase.DeleteAsset(assetPath);
                DestroyAllBindings(settings);
            }

            var fields = new List<BindingFieldInfo>();

            var namesHashSet = new HashSet<string>();

            IterateOverMeAndChildren(settings.gameObject, go =>
            {
                if (go.name.StartsWith(settings.bindTitle))
                    ProcessBind(settings, go, fields, namesHashSet, BindMode.Simple, true);

                if (go.name.StartsWith(settings.bindArrTitle))
                    ProcessBind(settings, go, fields, namesHashSet, BindMode.Array, true);
                
                if (go.name.StartsWith(settings.bindViewInstanceTitle))
                    ProcessBind(settings, go, fields, namesHashSet, BindMode.ViewInstance, true);

            }, o => !PrefabUtility.IsAnyPrefabInstanceRoot(o));

            var generator = new UiBinding
            {
                ClassName = GetBindingName(settings),
                Fields = fields,
                StateGroups = settings.stateGroups
            };

            var fullPath = GetFullPath(settings, false);
            if (!FileHelper.IsValidPath(fullPath, out var errorMessage))
            {
                log.Error(errorMessage);
                return;
            }

            FileHelper.WriteTextUtf8(fullPath, generator.TransformText());

            settings.lastGeneratedPath = fullPath;
            EditorUtility.SetDirty(settings);
            SavePrefab();

            AssetDatabase.Refresh();
            log.Info($"The script has been {(isUpdate ? "updated" : "generated")}. Path: {GetFullPath(settings, true)}");
        }

        private static void SavePrefab()
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            var prefabRoot = prefabStage.prefabContentsRoot;
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabStage.assetPath);
            PrefabStageUtility.GetCurrentPrefabStage().ClearDirtiness();
        }

        private static void IterateOverMeAndChildren(GameObject obj, Action<GameObject> iterationAct, Func<GameObject, bool> needToGoDeepFunc)
        {
            iterationAct.Invoke(obj);

            if (!needToGoDeepFunc.Invoke(obj))
                return;

            foreach (Transform child in obj.transform)
                IterateOverMeAndChildren(child.gameObject, iterationAct, needToGoDeepFunc);
        }

        private string GetFullPath(BindingGenerator settings, bool skipStart)
        {
            var baseBindingsPath = _globalSettings != null ? _globalSettings.baseBindingsPath : GlobalCycladeBindingsSettings.DefaultPath;
            var dataPath = !skipStart ? Path.GetFullPath(Application.dataPath) : "";
            return Path.Combine(dataPath, baseBindingsPath, settings.additionalFolderPath, $"{GetBindingName(settings)}.cs");
        }

        private static string GetBindingName(BindingGenerator settings) => $"{settings.name}{settings.bindPostfix}";

        private void ProcessBind(BindingGenerator settings, GameObject go, List<BindingFieldInfo> fields, HashSet<string> namesHashSet, BindMode bindMode, bool isRegister)
        {
            string title = "";
            if (bindMode == BindMode.Simple)
                title = settings.bindTitle;
            else if (bindMode == BindMode.Array)
                title = settings.bindArrTitle;
            else if (bindMode == BindMode.ViewInstance)
                title = settings.bindViewInstanceTitle;
            
            string determinedName = go.name
                .Replace(title, "")
                .Replace(" ", "");

            bool fullName = false;

            List<BindingInfo> bindingInfos;

            var bindingComponent = CycladeHelpers.FindComponent(go, c => c.GetType().Namespace == BindingConstants.GeneratedNameSpace, out _);
            if (bindingComponent != null)
            {
                bindingInfos = new List<BindingInfo>()
                {
                    new(bindingComponent.GetType(), "Bind"),
                };

                var isSpecifiedBinding = determinedName.Contains(':');
                if (isSpecifiedBinding)
                {
                    var split = determinedName.Split(':');
                    determinedName = split[0];
                    log.Warn($"Don't need \":\" in binding {go.name}. Removed(auto).", go);
                }

                fullName = true;
            }
            else
            {
                var isSpecifiedBinding = determinedName.Contains(':');
                string specifiedType = string.Empty;
                if (isSpecifiedBinding)
                {
                    var split = determinedName.Split(':');
                    determinedName = split[0];
                    specifiedType = split[1];
                }

                //only contains transform, we are dealing with a GameObject binding

                if (go.GetComponents<Component>().Length == 1 || (isSpecifiedBinding && specifiedType == "GameObject"))
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
                        //verify whether the default types include 'onlyInclude'.
                        if (bindingInfos.Any(q => q.Type.Name == specifiedType))
                        {
                            bindingInfos.RemoveAll(q => q.Type.Name != specifiedType);
                        }
                        else //if not, we are dealing with a non-default type for binding
                        {
                            fullName = true;
                            bindingInfos.Clear();
                            var foundComponent = FindComponent(go, specifiedType, out var foundComponents);
                            if (foundComponent != null)
                                bindingInfos.Add(new BindingInfo(foundComponent.GetType(), ""));
                            else
                                log.Warn($"Not found component with name {specifiedType}. Found components: {FormatFoundComponents(foundComponents)}. skip: {go.name}", go);
                        }
                    }

                    bindingInfos.RemoveAll(q => !go.TryGetComponent(q.Type, out _));

                    if (bindingInfos.Count == 0)
                    {
                        log.Warn($"Empty bind {determinedName}. skip: {go.name}", go);
                        return;
                    }
                }
            }

            SerializedObject serializedObject = default;
            if (!isRegister)
            {
                var component = settings.GetComponent(_bindingType);
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
                string finalType = "";
                if (bindMode == BindMode.Simple)
                    finalType = determinedType;
                else if (bindMode == BindMode.Array)
                    finalType = $"List<{determinedType}>";
                else if (bindMode == BindMode.ViewInstance)
                    finalType = $"ViewInstances<{determinedType}>";

                if (isRegister)
                    TryRegisterBindingField(go, fields, namesHashSet, finalName, finalType, bindMode);
                else
                    TryBindField(go, namesHashSet, serializedObject, finalName, determinedType, bindMode);
            }

            if (!isRegister)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(serializedObject.targetObject);
                serializedObject.Dispose();
            }
        }

        public void BindStatefulElements(BindingGenerator settings)
        {
            var component = settings.GetComponent(_bindingType);
            var serializedObject = new SerializedObject(component);

            var statefulElementsProp = serializedObject.FindProperty("statefulElements");
            if (statefulElementsProp != null)
            {
                statefulElementsProp.ClearArray();

                if (!_isClearMode)
                {
                    var list = new List<BaseStatefulElement>();
                    IterateOverMeAndChildren(settings.gameObject, go =>
                    {
                        var components = go.GetComponents<BaseStatefulElement>();
                        foreach (var statefulElement in components)
                            list.Add(statefulElement);
                    }, o => !PrefabUtility.IsAnyPrefabInstanceRoot(o));

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

        private void TryBindField(GameObject go, HashSet<string> namesHashSet, SerializedObject serializedObject, string finalName, string determinedType, BindMode bindMode)
        {
            var prop = serializedObject.FindProperty(finalName);
            if (prop == null)
            {
                log.Warn($"Property with name {finalName} not found");
                return;
            }

            Object foundValue = null;
            if (!_isClearMode)
            {
                if (determinedType == "GameObject")
                {
                    foundValue = go;
                }
                else
                {
                    foundValue = FindComponent(go, determinedType, out var foundComponents);
                    if (foundValue == null)
                        log.Warn($"Component with name {determinedType} not found in {go.name}. Found components: {FormatFoundComponents(foundComponents)}", go);
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
            => CycladeHelpers.FindComponent(go, c => c.GetType().Name == determinedType || c.GetType().FullName == determinedType, out foundComponents);

        private static void TryRegisterBindingField(GameObject go, List<BindingFieldInfo> fields, HashSet<string> namesHashSet, string determinedName, string determinedType, BindMode bindMode)
        {
            if (!namesHashSet.Add(determinedName))
            {
                if (bindMode == BindMode.Array)
                    log.Error($"We already have bind for {determinedName}. skip: {go.name}. Change name and re-generate.", go);
                return;
            }

            fields.Add(new BindingFieldInfo(determinedType, determinedName, bindMode));
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

        private static bool TryFindType(BindingGenerator settings, out Type type)
        {
            var types = CycladeHelpers.FindTypesWith(t => t.Name == GetBindingName(settings) && t.IsSubclassOf(typeof(MonoBehaviour)));
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