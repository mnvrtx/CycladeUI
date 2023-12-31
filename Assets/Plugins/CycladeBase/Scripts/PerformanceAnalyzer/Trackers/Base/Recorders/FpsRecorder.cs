using UnityEngine;

namespace CycladeBase.PerformanceAnalyzer.Trackers.Base.Recorders
{
    public class FpsRecorder : BaseRecorder
    {
        private const int FrameRange = 60;

        private int[] _fpsBuffer;
        private int _fpsBufferIndex;
        
        public FpsRecorder() : base("FPS", false)
        {
        }

        public override bool UpdateValue()
        {
            if (_fpsBuffer == null || _fpsBuffer.Length != FrameRange)
            {
                _fpsBuffer = new int[FrameRange];
                _fpsBufferIndex = 0;
            }

            int currentFPS = (int)(1f / Time.unscaledDeltaTime);

            _fpsBuffer[_fpsBufferIndex++] = currentFPS;
            if (_fpsBufferIndex >= FrameRange) _fpsBufferIndex = 0;

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
            return true;
        }

        public override void Dispose()
        {
        }
    }
}