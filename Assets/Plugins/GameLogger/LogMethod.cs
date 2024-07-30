using System;
using System.Diagnostics;

namespace Shared.Utils.Logging
{
    public class LogMethod : IDisposable
    {
        public static LogBuilder.Type LogBuilderDefaultType = LogBuilder.Type.JsonPretty;

        private readonly Log _log;
        private readonly string _tag;

        private bool _logFinish;
        private Stopwatch _stopwatch;
        private LogBuilder _infoBuilder;

        private readonly bool _isDebug;

        private string CTag => $"[{_log.TagTitle}] {_tag}";

        public LogMethod(Log log, string tag, bool isDebug = false)
        {
            _log = log;
            _tag = tag;
            _isDebug = isDebug;
            _log.PushTag2(_tag);
        }

        public LogBuilder ConfigureLogBuilder(LogBuilder.Type lbType = LogBuilder.Type.None)
        {
            if (lbType == LogBuilder.Type.None)
                lbType = LogBuilderDefaultType;

            _infoBuilder = new LogBuilder("METHOD INFO", lbType);
            if (!_logFinish)
                LogFinish();
            return _infoBuilder;
        }

        public LogMethod LogFinish(bool logStart = false)
        {
            _stopwatch = Stopwatch.StartNew();
            _logFinish = true;
            if (logStart)
            {
                var message = $"{CTag} method started.";
                if (_isDebug)
                    LogSystem.Debug(message);
                else
                    LogSystem.Info(message);
            }

            return this;
        }

        public void Dispose()
        {
            _log.PopTag2();

            if (_logFinish)
            {
                var info = _infoBuilder != null ? $"\n{_infoBuilder}" : string.Empty;
                var message = $"{CTag} method finished. Elapsed: {_stopwatch.ElapsedMilliseconds:N0}ms{info}";
                if (_isDebug)
                    LogSystem.Debug(message);
                else
                    LogSystem.Info(message);
            }
        }
    }
}