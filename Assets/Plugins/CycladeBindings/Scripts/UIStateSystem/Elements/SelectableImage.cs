using System.Collections.Generic;
using System.Linq;
using CycladeBase.Utils.Logging;
using CycladeBindings.UIStateSystem.Base;
using CycladeBindings.UIStateSystem.Models;
using UnityEngine;
using UnityEngine.UI;

namespace CycladeBindings.UIStateSystem.Elements
{
    [AddComponentMenu("UIStateSystem/" + nameof(SelectableImage))]
    public class SelectableImage : BaseSelectableState
    {
        private static readonly Log log = new(nameof(SelectableImage));

        public bool IsEnabled { get; private set; }

        [SerializeField] private bool specifiedAllStates = true;

        public List<ImageState> states;

        private Image _image;
        
        public override void Select(string stateName)
        {
            IsEnabled = stateName == "true";
            var imgState = states.FirstOrDefault(q => q.state == stateName);
            if (imgState != null)
            {
                if (UpdateImageCache())
                {
                    _image.sprite = imgState.image;
                    _image.color = imgState.color;
                }
                else
                    log.Warn($"Not found image in {gameObject.name}", gameObject);
            }
            else
            {
                if (specifiedAllStates)
                    log.Warn($"Not found state in {gameObject.name}: {stateName}", gameObject);
            }
        }

        private bool UpdateImageCache()
        {
            if (_image != null)
                return true;

            _image = GetComponent<Image>();
            return _image != null;
        }

        [ContextMenu("Flip active")]
        public void Flip()
        {
            Select(!IsEnabled);
        }
    }
}