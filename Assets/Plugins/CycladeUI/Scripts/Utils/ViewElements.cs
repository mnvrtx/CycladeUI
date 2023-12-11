using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CycladeUI.Utils
{
    [Serializable]
    public class ViewElements<T> where T : MonoBehaviour
    {
        [SerializeField] private T template;

        public IReadOnlyList<T> Elements => _instantiated;
        private readonly List<T> _instantiated = new();

        private bool _firstGet;

        public void Initialize()
        {
            template.SetActive(false);
            _firstGet = true;
        }

        public void Initialize(int count)
        {
            Initialize();
            Clear();

            for (int i = 0; i < count; i++)
                Get();
        }

        public T Get(Transform parent = null)
        {
            if (!_firstGet)
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