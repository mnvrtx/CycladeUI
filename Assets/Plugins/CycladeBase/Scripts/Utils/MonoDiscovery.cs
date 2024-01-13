using System;
using System.Collections.Generic;
using UnityEngine;

namespace CycladeBase.Utils
{
    [DefaultExecutionOrder(-99999)]
    public class MonoDiscovery : MonoBehaviour
    {
        private enum MonoDiscoveryAccessType
        {
            ByType,
            ByName,
        }

        private static readonly Dictionary<Type, Component> behaviours = new();
        private static readonly Dictionary<string, Component> behavioursByName = new();

        [SerializeField] private MonoDiscoveryAccessType accessType = MonoDiscoveryAccessType.ByType;
        [CycladeHelpBox("Not necessary if optionalComponent is null.")] public string stub;
        
        [SerializeField] private Component optionalComponent;
        [CycladeHelpBox("Leave null if you want the possibility of accessing all components.")] public string stub2;

        public static T Get<T>() where T : Component => (T)behaviours.GetValueOrDefault(typeof(T), null);
        public static T Get<T>(string name) where T : Component => (T)behavioursByName.GetValueOrDefault(name, null);

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
                        behavioursByName.Add(name, component);
                    else
                        behaviours.Add(component.GetType(), component);

                    _trackedComponents.Add((component, byName));
                }
            }
            else
            {
                if (accessType == MonoDiscoveryAccessType.ByName)
                    behavioursByName.Add(name, optionalComponent);    
                else
                    behaviours.Add(optionalComponent.GetType(), optionalComponent);
            }
            
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