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

        protected bool Equals(DebugSafeAreaSettings other)
        {
            return enabled == other.enabled && top.Equals(other.top) && left.Equals(other.left) && bot.Equals(other.bot) && right.Equals(other.right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DebugSafeAreaSettings)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(enabled, top, left, bot, right);
        }
    }
}