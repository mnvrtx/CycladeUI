using System;
using System.Runtime.CompilerServices;

namespace Shared.Utils.Logging
{
    public static class LogSystem
    {
        private static LogType _currentLevel;
        private static ILogger _logger;

        public static bool IsInitialized => _logger != null;
        
        public static void Initialize(LogType level, ILogger logger)
        {
            _logger = logger;
            SetLogLevel(level);
        }
        
        public static void SetLogLevel(LogType level)
        {
            if (_currentLevel == level)
                return;

            _currentLevel = level;
            _logger.Log(LogType.Info, $"[LogSystem] Set level to {level}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Trace(string s, object obj = null) => Log(LogType.Trace, s, obj);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(string s, object obj = null) => Log(LogType.Debug, s, obj);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Info(string s, object obj = null) => Log(LogType.Info, s, obj);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(string s, object obj = null) => Log(LogType.Warn, s, obj);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(string s, object obj = null) => Log(LogType.Error, s, obj);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fatal(string s, object obj = null) => Log(LogType.Fatal, s, obj);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Exception(Exception ex, object obj = null) => _logger.LogException(ex, obj);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is(LogType type) => !(type < _currentLevel);

        public static void Log(LogType type, string s, object obj = null)
        {       
            if (type < _currentLevel)
                return;

            _logger.Log(type, s, obj);
        }
    }
}