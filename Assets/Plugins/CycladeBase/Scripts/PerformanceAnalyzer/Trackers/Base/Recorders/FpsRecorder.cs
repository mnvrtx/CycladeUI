using UnityEngine;

namespace CycladeBase.PerformanceAnalyzer.Trackers.Base.Recorders
{
    public class FpsRecorder : BaseRecorder
    {
        private const int FrameRange = 60;
        private const int DontTrackDifference = 10;

        private int[] _fpsBuffer;
        private int _fpsBufferIndex;
        private int _lastFPS;
        
        public FpsRecorder() : base("FPS", false)
        {
        }

        public override void UpdateValue()
        {
            if (_fpsBuffer == null || _fpsBuffer.Length != FrameRange)
            {
                _fpsBuffer = new int[FrameRange];
                _fpsBufferIndex = 0;
                _lastFPS = -1;
            }

            int currentFPS = (int)(1f / Time.unscaledDeltaTime);

            if (DontTrackDifference != -1)
            {
                if (_lastFPS < 0 || Mathf.Abs(currentFPS - _lastFPS) <= DontTrackDifference)
                {
                    _fpsBuffer[_fpsBufferIndex++] = currentFPS;
                    if (_fpsBufferIndex >= FrameRange) _fpsBufferIndex = 0;
                }
            }

            _lastFPS = currentFPS;

            int sum = 0;
            int count = 0;
            foreach (int fps in _fpsBuffer)
            {
                if (fps != 0)
                {
                    sum += fps;
                    count++;
                }
            }

            FloatValue = count > 0 ? (sum / (float)count) : 0;
        }

        public override void Dispose()
        {
        }
    }
}