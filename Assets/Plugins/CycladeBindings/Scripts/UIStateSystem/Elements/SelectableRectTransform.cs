using System.Collections.Generic;
using System.Linq;
using CycladeBase.Utils;
using CycladeBase.Utils.Logging;
using CycladeBindings.UIStateSystem.Base;
using CycladeBindings.UIStateSystem.Models;
using UnityEngine;

namespace CycladeBindings.UIStateSystem.Elements
{
    [AddComponentMenu("UIStateSystem/" + nameof(SelectableRectTransform))]
    public class SelectableRectTransform : BaseSelectableState
    {
        private static readonly Log log = new(nameof(SelectableRectTransform));

        public bool IsEnabled { get; private set; }

        public List<RectTransformState> states;

        private RectTransform _rectTransform;
        
        public override void Select(string stateName)
        {
            IsEnabled = stateName == "true";
            var rtState = states.FirstOrDefault(q => q.state == stateName);
            if (rtState == null)
                rtState = states.FirstOrDefault(q => q.state == BindingConstants.AnyOtherState);
            
            if (rtState != null)
            {
                if (UpdateRectTransformState())
                    CycladeHelpers.SetRectTransformValues(_rectTransform, rtState.rectTransform);
                else
                    log.Warn($"Not found rectTransform in {gameObject.name}", gameObject);
            }
            else
            {
                log.Warn($"Not found state in {gameObject.name}: {stateName}", gameObject);
            }
        }

        private bool UpdateRectTransformState()
        {
            if (_rectTransform != null)
                return true;

            _rectTransform = GetComponent<RectTransform>();
            return _rectTransform != null;
        }

        [ContextMenu("Flip active")]
        public void Flip()
        {
            Select(!IsEnabled);
        }
    }
}