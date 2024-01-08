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
        [SerializeField] private Component component;

        public static T Get<T>() where T : Component => (T)behaviours.GetValueOrDefault(typeof(T), null);
        public static T Get<T>(string name) where T : Component => (T)behavioursByName.GetValueOrDefault(name, null);

        private void Awake()
        {
            if (accessType == MonoDiscoveryAccessType.ByType)
                behaviours.Add(component.GetType(), component);
            else
                behavioursByName.Add(name, component);
        }

        private void OnDestroy()
        {
            if (accessType == MonoDiscoveryAccessType.ByType)
                behaviours.Remove(component.GetType());
            else
                behavioursByName.Remove(name);
        }
    }
}