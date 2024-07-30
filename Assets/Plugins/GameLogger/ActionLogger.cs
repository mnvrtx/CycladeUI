using System;

namespace Shared.Utils.Logging
{
    public class ActionLogger : ILogger
    {
        private readonly Action<LogType, string> _onLog;

        public ActionLogger(Action<LogType, string> onLog)
        {
            _onLog = onLog;
        }

        public void Log(LogType logType, string msg, object _ = null) => _onLog.Invoke(logType, msg);
        public void LogException(Exception ex, object _ = null) => _onLog.Invoke(LogType.Error, ex.ToString());
    }
}