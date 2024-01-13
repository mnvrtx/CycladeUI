using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace CycladeBase.Utils.DebugDots
{
    public class DebugDotsView : MonoBehaviourSingleton<DebugDotsView>
    {
        public float defaultDotSize = 0.1f;
        public int defaultMs = 500;

        private readonly Queue<DotData> _debugColoredDots = new();
        private readonly List<DotData> _dots = new();
        private long _currentStepIdx;

        [Conditional("UNITY_EDITOR")]
        public void DisplayDot(Vector3 vec, Vector3 direction = default, float size = 0, Color c = default, TimeSpan time = default)
        {
            if (c == default)
                c = Color.blue;

            var ms = time != default ? time.TotalMilliseconds : defaultMs;
            var ticks = (int)(ms / 1000f / Time.fixedDeltaTime);
            var dotSize = size == 0 ? defaultDotSize : size;

            _debugColoredDots.Enqueue(new DotData(vec, direction, c, _currentStepIdx, dotSize, ticks));
        }

#if UNITY_EDITOR
        private void FixedUpdate()
        {
            if (_dots.Count == 0 && _debugColoredDots.Count == 0)
            {
                return;
            }

            _currentStepIdx++;

            for (var i = 0; i < _dots.Count; i++)
            {
                var dot = _dots[i];
                if (dot.StepIdx <= _currentStepIdx - dot.Ticks)
                    _dots.FastRemoveAndDecrI(ref i);
            }

            while (_debugColoredDots.Count > 0)
            {
                var data = _debugColoredDots.Dequeue();
                _dots.Add(data);
            }

            for (int i = 0; i < _dots.Count; i++)
            {
                var dot = _dots[i];

                dot.Color = dot.Color
                    .WithAlpha(Mathf.Clamp01(1f - (float)(_currentStepIdx - dot.StepIdx) / dot.Ticks));
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var dot in _dots)
            {
                Gizmos.color = dot.Color;
                Gizmos.DrawSphere(dot.Position, dot.Color.EqualsWithoutAlpha(Color.red) ? defaultDotSize * 1.3f : defaultDotSize);
                if (dot.Direction != Vector3.zero)
                    Gizmos.DrawLine(dot.Position, dot.Position + dot.Direction);
            }
        }
#endif
    }
}