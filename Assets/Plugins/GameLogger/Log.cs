using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Shared.Utils.Logging
{
    public class Log
    {
        private string _tag;
        private readonly IDebugControlProvider _debugControlProvider;

        public readonly string TagTitle;
        private readonly Stack<string> _tag2Stack = new();

        public Log(string tagTitle, IDebugControlProvider debugControlProvider = null)
        {
            TagTitle = tagTitle;
            _debugControlProvider = debugControlProvider;
            UpdateTag();
        }

        public void PushTag2(string tag)
        {
            _tag2Stack.Push(tag);
            UpdateTag();
        }

        public void PopTag2()
        {
            _tag2Stack.Pop();
            UpdateTag();
        }

        private void UpdateTag()
        {
            if (_tag2Stack.Count == 0)
                _tag = $"[{TagTitle}] ";
            else
                _tag = $"[{TagTitle}.{_tag2Stack.Peek()}] ";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PrintData(object o, string name = "Data") => Debug($"{_tag} {name}: {JsonConvert.SerializeObject(o)}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PrintDataPretty(object o, string name = "Data") => Debug($"{_tag} {name}: {JsonConvert.SerializeObject(o, Formatting.Indented)}");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Message(LogType logType, string s, object obj = null) => LogSystem.Log(logType, $"{_tag}{s}", obj);

        public void Trace(string s, bool force = false, object obj = null)
        {
            if (!force && (!_debugControlProvider?.IsDebug ?? false))
                return;

            LogSystem.Trace($"{_tag}{s}", obj);
        }

        public void Debug(string s, bool force = false, object obj = null)
        {
            if (!force && (!_debugControlProvider?.IsDebug ?? false))
                return;

            LogSystem.Debug($"{_tag}{s}", obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Info(string s, object obj = null) => LogSystem.Info($"{_tag}{s}", obj);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Warn(string s, object obj = null) => LogSystem.Warn($"{_tag}{s}", obj);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Error(string s, object obj = null) => LogSystem.Error($"{_tag}{s}", obj);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Exception(Exception ex, object obj = null) => LogSystem.Exception(ex, obj);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fatal(string s, object obj = null) => LogSystem.Fatal($"{_tag}{s}", obj);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Conditional("UNITY_EDITOR")]
        public void ClientWatch(string name, object obj) => FieldsWatcher.ClientWatch($"{_tag}{name}", obj);
    }
}