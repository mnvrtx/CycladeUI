using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CycladeUI.UIBindingSystem;
using CycladeUI.Utils;
using CycladeUI.Utils.Logging;
using CycladeUIEditor.Utils;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CycladeUIEditor.UIBindingSystem
{
    [CustomEditor(typeof(BindingGenerator))]
    public class BindingGeneratorEditor : Editor
    {
        private static readonly Log log = new(nameof(BindingGeneratorEditor));

        [NonSerialized] private Type _bindingType;

        public override void OnInspectorGUI()
        {
            var settings = (BindingGenerator)target;

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Bindings Path: ", GUILayout.Width(150));
            settings.generatedPath = EditorGUILayout.TextField(settings.generatedPath);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Bindings Name: ", GUILayout.Width(150));
            settings.generatedName = EditorGUILayout.TextField(settings.generatedName);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Generate"))
                Generate(settings);

            if (GUILayout.Button("TryFindAndAddComponent"))
                TryFindAndProcessComponent(settings, true);

            if (GUILayout.Button("TryRemoveComponent"))
                TryFindAndProcessComponent(settings, false);

            if (GUILayout.Button("SetBindings"))
                SetBindings(settings);
        }

        private void SetBindings(BindingGenerator settings)
        {
            if (!TryFindType(settings, out _bindingType))
                return;

            var namesHashSet = new HashSet<string>();

            settings.transform.IterateOnMeAndChildren<Transform>(t =>
            {
                if (t.name.StartsWith(settings.bindTitle))
                    ProcessBind(settings, t, null, namesHashSet, false, false);

                if (t.name.StartsWith(settings.bindArrTitle))
                    ProcessBind(settings, t, null, namesHashSet, true, false);
            });
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
                ClassName = settings.generatedName,
                Fields = fields,
            };

            var fullPathClient = Path.GetFullPath(Path.Combine(Application.dataPath, settings.generatedPath, $"{settings.generatedName}.cs"));
            FileHelper.WriteTextUtf8(fullPathClient, generator.TransformText());
            AssetDatabase.Refresh();
            log.Info("generated");
        }

        private void ProcessBind(BindingGenerator settings, Transform transform, List<BindingFieldInfo> fields, HashSet<string> namesHashSet, bool isArr, bool isRegister)
        {
            string determinedName = transform.name
                .Replace(!isArr ? settings.bindTitle : settings.bindArrTitle, "")
                .Replace(" ", "");

            var types = new[]
            {
                (typeof(TMP_Text), "Txt"),
                (typeof(Text), "Txt"),
                (typeof(Image), "Img"),
                (typeof(Button), "Btn"),
            }.ToList();

            if (determinedName.Contains(':'))
            {
                var split = determinedName.Split(':');
                determinedName = split[0];
                var onlyInclude = split[1];

                types.RemoveAll(q => q.Item2 != onlyInclude);
            }

            types.RemoveAll(q => !transform.TryGetComponent(q.Item1, out _));

            if (types.Count == 0)
            {
                log.Warn($"empty bind {determinedName}. skip: {transform.gameObject.GetFullPath()}");
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
                var determinedType = tuple.Item1.Name;
                var finalType = !isArr ? determinedType : $"List<{determinedType}>";
                if (isRegister)
                {
                    TryRegisterBindingField(transform, fields, namesHashSet, finalName, finalType, isArr);
                }
                else
                {
                    TryBindField(transform, namesHashSet, serializedObject, finalName, determinedType);
                }
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

            var components = transform.GetComponents<Component>();
            Component foundComponent = default;
            foreach (var component in components)
            {
                if (component.GetType().Name == determinedType)
                {
                    foundComponent = component;
                    break;
                }
            }

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

        private void TryFindAndProcessComponent(BindingGenerator settings, bool isAdd)
        {
            if (!TryFindType(settings, out var type))
                return;

            if (isAdd)
            {
                if (!settings.gameObject.TryGetComponent(type, out _))
                {
                    Undo.AddComponent(settings.gameObject, type);
                    log.Info($"{type.Name} added");
                }
                else
                {
                    log.Warn($"{type.Name} already added");
                }
            }
            else
            {
                if (settings.gameObject.TryGetComponent(type, out var component))
                {
                    Undo.DestroyObjectImmediate(component);
                    log.Info($"{type.Name} removed");
                }
                else
                {
                    log.Warn($"{type.Name} already removed");
                }
            }
        }

        private static bool TryFindType(BindingGenerator settings, out Type type)
        {
            var types = EditorHelpers.FindTypesWith(t => t.Name == settings.generatedName && t.IsSubclassOf(typeof(MonoBehaviour)));
            if (types.Count == 0)
            {
                log.Warn($"Not found {settings.generatedName}");
                type = default;
                return false;
            }

            if (types.Count > 1)
                log.Warn($"More than one {settings.generatedName}. Just picking the first one.");

            type = types[0];
            return true;
        }
    }
}