using UnityEngine;

namespace CycladeBase.Utils.DebugDots
{
    public class DotData
    {
        public Vector3 Position;
        public Vector3 Direction;
        public Color Color;
        public long StepIdx;
        public int Ticks;
        public float Radius;

        public DotData(Vector3 position, Vector3 direction, Color color, long stepIdx, float radius, int ticks = -1)
        {
            Position = position;
            Direction = direction;
            Color = color;
            StepIdx = stepIdx;
            Radius = radius;
            Ticks = ticks;
        }
    }
}