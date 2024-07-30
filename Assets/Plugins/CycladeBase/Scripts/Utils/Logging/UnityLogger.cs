using System;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Shared.Utils.Logging
{
    public class UnityLogger : ILogger
    {
        public void Log(LogType logType, string msg, object obj = null)
        {
            switch (logType) {
                case LogType.Trace:
                    Debug.Log($"[T] {msg}", (Object)obj);
                    break;
                case LogType.Debug:
                    Debug.Log($"[D] {msg}", (Object)obj);
                    break;
                case LogType.Info:
                    Debug.Log(msg, (Object)obj);
                    break;
                case LogType.Warn:
                    Debug.LogWarning(msg, (Object)obj);
                    break;
                case LogType.Error:
                    Debug.LogError(msg, (Object)obj);
                    break;
                case LogType.Fatal:
                    Debug.LogError($"[FATAL] {msg}", (Object)obj);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public void LogException(Exception ex, object obj = null)
        {
            Debug.LogException(ex, (Object)obj);
        }
    }
}