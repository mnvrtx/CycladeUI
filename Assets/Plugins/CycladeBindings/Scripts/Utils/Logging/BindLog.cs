using System;

namespace CycladeBindings.Utils.Logging
{
    public class BindLog
    {
        public enum LogType
        {
            Log,
            Warn,
            Error,
            Exception,
        }

        private readonly string _tag;

        public readonly string TagTitle;

        public BindLog(string tagTitle)
        {
            TagTitle = tagTitle;
            _tag = $"[{TagTitle}] ";
        }

        public void PrintData(string o, string name = "Data") => LogInternal(LogType.Log, $"[D] {_tag} {name}: {o}");

        public void Trace(string s) => LogInternal(LogType.Log, $"[T] {_tag}{s}");

        public void Debug(string s, bool force = false) => LogInternal(LogType.Log, $"[D] {_tag}{s}");

        public void Info(string s) => LogInternal(LogType.Log, $"{_tag}{s}");

        public void Warn(string s) => LogInternal(LogType.Warn, $"{_tag}{s}");

        public void Error(string s) => LogInternal(LogType.Error, $"{_tag}{s}");

        public void Fatal(string s) => LogInternal(LogType.Error, $"[FATAL] {_tag}{s}");

        public void Exception(Exception ex) => LogInternal(LogType.Exception, ex);

        public void LogInternal(LogType type, object info, bool forceInstantPrint = false)
        {
            switch (type)
            {
                case LogType.Log:
                    UnityEngine.Debug.Log(info);
                    break;
                case LogType.Warn:
                    UnityEngine.Debug.LogWarning(info);
                    break;
                case LogType.Error:
                    UnityEngine.Debug.LogError(info);
                    break;
                case LogType.Exception:
                    UnityEngine.Debug.LogException((Exception)info);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}