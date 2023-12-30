using System;
using UnityEngine;

namespace CycladeBindings.UIStateSystem.Models
{
    [Serializable]
    public class ImageState : BaseUIElementState
    {
        public Sprite image;
        public Color color = Color.white;
    }
}