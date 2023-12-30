using System;
using CycladeBase.Utils;
using UnityEngine.Events;

namespace CycladeBindings.UIStateSystem.Models
{
    [Serializable]
    public class UnityEventState : BaseUIElementState
    {
        public UnityEvent unityEvent;
    }
}