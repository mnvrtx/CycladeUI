using System;
using System.Collections.Generic;
using System.Linq;

namespace CycladeUI.Utils.Logging
{
    public class Log
    {
        public const string DebugKey = "CycladeUIDebug";
        public static readonly Cache<bool> IsDebug = new(() =>
        {
#if UNITY_EDITOR
            return UnityEditor.SessionState.GetBool(DebugKey, false);
#else
            return false;
#endif
        });

        public enum LogType
        {
            Log,
            Warn,
            Error,
            Exception,
        }

        private readonly string _tag;

        public bool QueueMode;
        public readonly string TagTitle;
        private Queue<string> _queue = new();

        public Log(string tagTitle, bool queueMode = false)
        {
            TagTitle = tagTitle;
            _tag = $"[{TagTitle}] ";
            QueueMode = queueMode;
        }

        public void PrintData(string o, string name = "Data")
        {
            if (!IsDebug)
                return;

            LogInternal(LogType.Log, $"[D] {_tag} {name}: {o}");
        }

        public void Trace(string s)
        {
            if (!IsDebug)
                return;

            LogInternal(LogType.Log, $"[T] {_tag}{s}");
        }

        public void Debug(string s)
        {
            if (!IsDebug)
                return;

            LogInternal(LogType.Log, $"[D] {_tag}{s}");
        }

        public void Info(string s) => LogInternal(LogType.Log, $"{_tag}{s}");

        public void Warn(string s) => LogInternal(LogType.Warn, $"{_tag}{s}");

        public void Error(string s) => LogInternal(LogType.Error, $"{_tag}{s}");

        public void Fatal(string s) => LogInternal(LogType.Error, $"[FATAL] {_tag}{s}");

        public void Exception(Exception ex) => LogInternal(LogType.Exception, ex);

        public void LogInternal(LogType type, object info, bool forceInstantPrint = false)
        {
            if (!QueueMode || forceInstantPrint)
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
            else
            {
                _queue.Enqueue($"{type}^^^{info}");
            }
        }

        public void SaveQueue()
        {
#if UNITY_EDITOR
            UnityEditor.SessionState.SetString($"CycladeUISavedLog_{TagTitle}", _queue.Count > 0 ? _queue.Aggregate((q, q2) => $"{q}|||{q2}") : "");
#endif
        }

        public void LoadQueue()
        {
#if UNITY_EDITOR
            var str = UnityEditor.SessionState.GetString($"CycladeUISavedLog_{TagTitle}", "");
            if (string.IsNullOrEmpty(str))
                return;

            var split = str.Split("|||");
            _queue = new Queue<string>();
            foreach (var s in split)
                _queue.Enqueue(s);
#endif
        }

        public void DequeueAllLogs(string deferredTitle = "Deferred")
        {
            var haveQueue = _queue.Count > 0;
            if (haveQueue && IsDebug)
                LogInternal(LogType.Log, $"[D] {_tag}-- {deferredTitle} logs queue:", true);

            while (_queue.Count > 0)
            {
                var info = _queue.Dequeue();
                var split = info.Split("^^^");
                var logType = Enum.Parse<LogType>(split[0]);
                LogInternal(logType, split[1], true);
            }

            if (haveQueue && IsDebug)
                LogInternal(LogType.Log, $"[D] {_tag}-- {deferredTitle} logs queue printed", true);
        }

        public void CleanQueue()
        {
            _queue.Clear();
        }
    }
}