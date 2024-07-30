using System;
using UnityEngine;

namespace CycladeBindings.UIStateSystem.Models
{
    [Serializable]
    public class TextColorState : BaseUIElementState
    {
        public Color color = Color.white;
    }
}