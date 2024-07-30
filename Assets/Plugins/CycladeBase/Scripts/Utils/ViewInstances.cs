using System;
using System.Collections.Generic;
using Solonity.View.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CycladeBase.Utils
{
    [Serializable]
    public class ViewInstances<T> where T : Component
    {
        [SerializeField] private T template;

        [NonSerialized] public Transform Holder;
        public int Count => Instances.Count;
        public IReadOnlyList<T> Instances => _instantiated;
        private readonly List<T> _instantiated = new();

        private bool _initialized;

        public void Initialize(int count)
        {
            Initialize();

            for (int i = 0; i < count; i++)
                GetNew();
        }

        public void Initialize()
        {
            Clear();
            template.SetActive(false);
            Holder = template.transform.parent;
            _initialized = true;
        }

        public T GetNew(Transform parent = null)
        {
            if (!_initialized)
                Initialize();

            var component = Object.Instantiate(template, parent == null ? Holder : parent);
            component.gameObject.name = $"{template.name}_clone_{_instantiated.Count}";
            component.SetActive(true);
            _instantiated.Add(component);
            return component;
        }

        public void Remove(T toRemove)
        {
            if (_instantiated.Remove(toRemove))
                Object.Destroy(toRemove.gameObject);
        }

        public void Clear()
        {
            template.SetActive(false);

            for (int i = _instantiated.Count - 1; i >= 0; i--)
            {
                var element = _instantiated[i];
                Object.Destroy(element.gameObject);
                _instantiated.RemoveAt(i);
            }
        }
    }
}