using CycladeUI.Popups.System;
using UnityEngine;
using UnityEngine.UI;

namespace CycladeUIExample.Popups
{
    public class HeavyPopup : BasePopup
    {
        [SerializeField] private RawImage rawImage;
        [SerializeField] private Camera renderCamera;

        private RenderTexture _rt;

        private void Awake()
        {
            _rt = new RenderTexture(1024, 1024, 32);
            renderCamera.targetTexture = _rt;
            rawImage.texture = _rt;
        }

        private void OnDestroy()
        {
            Destroy(_rt);
        }
    }
}