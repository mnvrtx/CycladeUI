using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace CycladeUI.Utils.Logging
{
    public class Log
    {
        private const bool IsDebug = true;
        private const bool IsTrace = true;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PrintData(string o, string name = "Data")
        {
            if (!IsDebug)
                return;

            LogInternal(LogType.Log, $"[D] {_tag} {name}: {o}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Trace(string s)
        {
            if (!IsTrace)
                return;

            LogInternal(LogType.Log, $"[T] {_tag}{s}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Debug(string s)
        {
            if (!IsDebug)
                return;
                    
            LogInternal(LogType.Log, $"[D] {_tag}{s}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Info(string s) => LogInternal(LogType.Log, $"{_tag}{s}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Warn(string s) => LogInternal(LogType.Warn, $"{_tag}{s}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Error(string s) => LogInternal(LogType.Error, $"{_tag}{s}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fatal(string s) => LogInternal(LogType.Error, $"[FATAL] {_tag}{s}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            SessionState.SetString($"savedLog_{TagTitle}", _queue.Aggregate((q, q2) => $"{q}|||{q2}"));
        }

        public void LoadQueue()
        {
            var str = SessionState.GetString($"savedLog_{TagTitle}", "");
            if (string.IsNullOrEmpty(str))
                return;

            var split = str.Split("|||");
            _queue = new Queue<string>();
            foreach (var s in split)
                _queue.Enqueue(s);
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
                LogInternal(Enum.Parse<LogType>(split[0]), split[1], true);
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