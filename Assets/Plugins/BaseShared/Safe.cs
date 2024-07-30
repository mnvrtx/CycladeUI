using System;
using Shared.Utils.Logging;

namespace BaseShared
{
    public static class Safe
    {
        private static readonly Log myLog = new(nameof(Safe));

        public static void TryCatch(this Action a, Log eLog = null)
        {
            try
            {
                a?.Invoke();
            }
            catch (Exception e)
            {
                var log = eLog ?? myLog;
                log.Exception(e);
            }
        }

        public static void TryCatch<T>(this Action<T> a, T arg1, Log eLog = null)
        {
            try
            {
                a?.Invoke(arg1);
            }
            catch (Exception e)
            {
                var log = eLog ?? myLog;
                log.Exception(e);
            }
        }

        public static void TryCatch<T, T2>(this Action<T, T2> a, T arg1, T2 arg2, Log eLog = null)
        {
            try
            {
                a?.Invoke(arg1, arg2);
            }
            catch (Exception e)
            {
                var log = eLog ?? myLog;
                log.Exception(e);
            }
        }

        public static void TryCatch<T, T2, T3>(this Action<T, T2, T3> a, T arg1, T2 arg2, T3 arg3, Log eLog = null)
        {
            try
            {
                a?.Invoke(arg1, arg2, arg3);
            }
            catch (Exception e)
            {
                var log = eLog ?? myLog;
                log.Exception(e);
            }
        }
    }
}