using System;
using UnityEngine;

namespace CycladeBindings.UIStateSystem.Models
{
    [Serializable]
    public class ImageState
    {
        public string state;
        public Sprite image;
        public Color color = Color.white;
    }
}