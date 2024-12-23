using System;
using System.Collections.Generic;
using BaseShared.DomainReload;
using Shared.Utils;
using Shared.Utils.Logging;
using Solonity.View.Utils;
using UnityEngine;

namespace CycladeBase.Utils
{
    [DefaultExecutionOrder(-99999)]
    public class MonoDiscovery : MonoBehaviour
    {
        private static readonly Log log = new(nameof(MonoDiscovery), CycladeDebugInfo.I);

        private enum MonoDiscoveryAccessType
        {
            ByType,
            ByName,
        }

        private static Dictionary<Type, Component> behaviours = new();
        private static Dictionary<string, Component> behavioursByName = new();

        [ExecuteOnReload]
        public static void ResetCache()
        {
            behaviours = new Dictionary<Type, Component>();
            behavioursByName = new Dictionary<string, Component>();
        }

        [SerializeField] private MonoDiscoveryAccessType accessType = MonoDiscoveryAccessType.ByType;
        [CycladeHelpBox("Not necessary if optionalComponent is null.")] public string stub;
        
        [SerializeField] private Component optionalComponent;
        [CycladeHelpBox("Leave null if you want the possibility of accessing all components.")] public string stub2;

        public static T RegisterGlobally<T>(Action<T> onInit = null, string optionalName = "") where T : Component
        {
            var comp = RegisterForScene(onInit, optionalName);
            DontDestroyOnLoad(comp.gameObject);
            comp.name = $"[GLOBAL] " + comp.name; 
            return comp;
        }

        public static T RegisterForScene<T>(Action<T> onInit = null, string optionalName = "") where T : Component
        {
            var haveName = !optionalName.IsNullOrEmpty();
            var go = new GameObject($"Service_{(haveName ? optionalName : typeof(T).Name)}");
            go.SetActive(false);
            var comp = go.AddComponent<T>();
            onInit?.Invoke(comp);
            var monoDiscovery = go.AddComponent<MonoDiscovery>();
            monoDiscovery.accessType = haveName ? MonoDiscoveryAccessType.ByName : MonoDiscoveryAccessType.ByType;
            monoDiscovery.optionalComponent = comp;
            go.SetActive(true);
            return comp;
        }
        
        public static void TryToExecute<T>(Action<T> action, bool warnIfNotExecuted = true) where T : Component
        {
            var c = Get<T>();
            if (c == null)
            {
                if (warnIfNotExecuted)
                    log.Warn($"not found component with \"{typeof(T)}\" type. not executed action");
                return;
            }
            
            action.Invoke(c);
        }
        
        public static void TryToExecute<T>(string name, Action<T> action, bool warnIfNotExecuted = true) where T : Component
        {
            var c = Get<T>(name, false);
            if (c == null)
            {
                if (warnIfNotExecuted)
                    log.Warn($"not found component with \"{name}\" name. not executed action");
                return;
            }
            
            action.Invoke(c);
        }
        
        public static T GetOrCreate<T>(Action<T> onCreate = null) where T : Component
        {
            var found = behaviours.TryGetValue(typeof(T), out var value);
            if (!found)
            {
                var instance = RegisterGlobally<T>();
                value = instance;
                onCreate?.Invoke(instance);
            }
            return (T)value;
        }

        public static T Get<T>(bool errorIfNotFound = true) where T : Component
        {
            var found = behaviours.TryGetValue(typeof(T), out var value);
            if (errorIfNotFound && !found)
                log.Error($"not found component with \"{typeof(T)}\" type");
            return (T)value;
        }

        public static T Get<T>(string name, bool errorIfNotFound = true) where T : Component
        {
            var found = behavioursByName.TryGetValue(name, out var value);
            if (errorIfNotFound && !found)
                log.Error($"not found component with \"{name}\" name");
            return (T)value;
        }

        private string GetId() => gameObject.GetFullPath();

        private readonly List<(Component component, bool byName)> _trackedComponents = new();

        private void Awake()
        {
            if (optionalComponent == null)
            {
                var components = GetComponents<Component>();
                foreach (var component in components)
                {
                    if (component == this)
                        continue;

                    var byName = component is Transform;
                    if (byName)
                    {
                        var isAdded = behavioursByName.TryAdd(name, component);
                        if (!isAdded)
                        {
                            log.Error($"{GetId()}: component with name \"{name}\" already added", gameObject);
                            continue;
                        }
                    }
                    else
                    {
                        var isAdded = behaviours.TryAdd(component.GetType(), component);
                        if (!isAdded)
                        {
                            log.Error($"{GetId()}: component with type \"{component.GetType()}\" already added", gameObject);
                            continue;
                        }
                    }

                    _trackedComponents.Add((component, byName));
                }
            }
            else
            {
                if (accessType == MonoDiscoveryAccessType.ByName)
                {
                    var isAdded = behavioursByName.TryAdd(name, optionalComponent);
                    if (!isAdded)
                        log.Error($"{GetId()}: component with name \"{name}\" already added", gameObject);
                }
                else
                {
                    var isAdded = behaviours.TryAdd(optionalComponent.GetType(), optionalComponent);
                    if (!isAdded)
                        log.Error($"{GetId()}: component with type \"{optionalComponent.GetType()}\" already added", gameObject);
                }
            }
        }
        
        [ContextMenu("Print Tracked Components")]
        private void PrintTrackedComponents()
        {
            foreach (var item in _trackedComponents) 
                Debug.Log($"Component: {item.component}, ByName: {item.byName}");
        }

        private void OnDestroy()
        {
            if (optionalComponent == null)
            {
                foreach (var tuple in _trackedComponents)
                {
                    if (tuple.byName)
                        behavioursByName.Remove(name);
                    else
                        behaviours.Remove(tuple.component.GetType());
                }
            }
            else
            {
                if (accessType == MonoDiscoveryAccessType.ByName)
                    behavioursByName.Remove(name);
                else
                    behaviours.Remove(optionalComponent.GetType());
            }
        }
    }
}