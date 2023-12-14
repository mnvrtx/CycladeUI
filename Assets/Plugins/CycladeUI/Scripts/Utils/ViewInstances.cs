using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CycladeUI.Utils
{
    [Serializable]
    public class ViewInstances<T> where T : MonoBehaviour
    {
        [SerializeField] private T template;

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
            _initialized = true;
        }

        public T GetNew(Transform parent = null)
        {
            if (!_initialized)
                Initialize();
            
            var component = Object.Instantiate(template, parent == null ? template.transform.parent : parent);
            component.gameObject.name = $"{template.name}_clone_{_instantiated.Count}";
            component.SetActive(true);
            _instantiated.Add(component);
            return component;
        }

        public void Clear()
        {
            for (int i = _instantiated.Count - 1; i >= 0; i--) {
                var element = _instantiated[i];
                Object.Destroy(element.gameObject);
                _instantiated.RemoveAt(i);
            }
        }
    }
}