using System.Diagnostics;
using Newtonsoft.Json;

namespace Shared.Utils.Logging
{
    public static class LogExtension
    {
        private static readonly Log internalLog = new(nameof(LogExtension));

        [Conditional("UNITY_EDITOR")]
        public static void ClientWatch(this object obj, Log log = null)
        {
            var l = log ?? internalLog;
            l.ClientWatch(obj.GetType().Name, JsonConvert.SerializeObject(obj));
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void ClientWatch(this object obj, string title, Log log = null)
        {
            var l = log ?? internalLog;
            l.ClientWatch(title, JsonConvert.SerializeObject(obj));
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void ClientWatchStr(this object obj, string title, Log log = null)
        {
            var l = log ?? internalLog;
            l.ClientWatch(title, obj);
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void ClientWatchPretty(this object obj, Log log = null)
        {
            var l = log ?? internalLog;
            l.ClientWatch(obj.GetType().Name, JsonConvert.SerializeObject(obj, Formatting.Indented));
        }
    }
}