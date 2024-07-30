using System.Collections.Generic;
using System.Linq;
using Shared.Utils.Logging;
using CycladeBindings.UIStateSystem.Models;
using UnityEngine;

namespace CycladeBindings.UIStateSystem.Base
{
    public abstract class BaseSelectableState<TElement, TState> : BaseGroupableState 
        where TElement : Object
        where TState : BaseUIElementState
    {
        private static readonly Log log = new(nameof(BaseSelectableState<TElement, TState>), CycladeDebugInfo.I);

        public bool IsEnabled { get; private set; }

        public List<TState> states;

        private TElement _element;

        private bool _isGameObjectChecked;
        private bool _isGameObject;

        public override void Select(string stateName)
        {
            IsEnabled = stateName == "true";
            var state = states.FirstOrDefault(q => q.state == stateName);
            if (state == null)
                state = states.FirstOrDefault(q => q.state == BindingConstants.AnyOtherState);

            if (state != null)
            {
                if (UpdateElementCache())
                    OnSelected(_element, state);
                else
                    log.Warn($"Not found element in {gameObject.name}", gameObject);
            }
            else
            {
                log.Warn($"Not found state in {gameObject.name}: {stateName}. You can add {BindingConstants.AnyOtherState}.", gameObject);
            }
        }

        protected abstract void OnSelected(TElement element, TState state);

        private bool UpdateElementCache()
        {
            if (_element != null)
                return true;

            if (!_isGameObjectChecked)
            {
                _isGameObject = typeof(TElement) == typeof(GameObject);
                _isGameObjectChecked = true;
            }
            
            if (_isGameObject)
            {
                _element = (TElement)(Object)gameObject;
                return true;
            }

            _element = GetComponent<TElement>();
            return _element != null;
        }
    }
}