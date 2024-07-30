using System;
using CycladeBase.Utils;

namespace CycladeBase.Models
{
    [Serializable]
    public class DebugSafeAreaSettings
    {
        public bool enabled;

        public float top;
        public float left;
        public float bot;
        public float right;

        public DebugSafeAreaSettings(DebugSafeAreaSettings other)
        {
            enabled = other.enabled;
            
            top = other.top;
            left = other.left;
            bot = other.bot;
            right = other.right;
        }

        public bool Equals(DebugSafeAreaSettings other)
        {
            if (other == null)
                return false;
            return enabled == other.enabled && top.Equals(other.top) && left.Equals(other.left) && bot.Equals(other.bot) && right.Equals(other.right);
        }
    }
}