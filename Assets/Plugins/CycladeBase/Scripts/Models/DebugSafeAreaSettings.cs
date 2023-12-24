using System;
using CycladeBase.Utils;

namespace CycladeBase.Models
{
    [Serializable]
    public class DebugSafeAreaSettings
    {
        [CycladeHelpBox("If you enable safeArea, the mock, if present, will turn off.")] public string stub2;
        public bool enabled;

        public float top;
        public float left;
        public float bot;
        public float right;
    }
}