using System;

namespace CycladeUI.Models
{
    [Serializable]
    public class DebugSafeAreaSettings
    {
        public bool enabled;

        public float top;
        public float left;
        public float bot;
        public float right;
    }
}