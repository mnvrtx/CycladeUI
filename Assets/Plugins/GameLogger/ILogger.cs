using System;

namespace Shared.Utils.Logging
{
    public interface ILogger
    {
        void Log(LogType logType, string msg, object obj = null);
        void LogException(Exception ex, object obj = null);
    }
}