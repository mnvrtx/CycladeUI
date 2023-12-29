using System;
using CycladeBase.Utils.Logging;

namespace CycladeBase.Utils
{
    public static class SafeInvokeHelper
    {
        public static void SafeInvoke(this Action action, Log log)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                log.Exception(e);
            }
        }

        public static void SafeInvoke<T>(this Action<T> action, T arg1, Log log)
        {
            try
            {
                action?.Invoke(arg1);
            }
            catch (Exception e)
            {
                log.Exception(e);
            }
        }

        public static void SafeInvoke<T, T2>(this Action<T, T2> action, T arg1, T2 arg2, Log log)
        {
            try
            {
                action?.Invoke(arg1, arg2);
            }
            catch (Exception e)
            {
                log.Exception(e);
            }
        }

        public static void SafeInvoke<T, T2, T3>(this Action<T, T2, T3> action, T arg1, T2 arg2, T3 arg3, Log log)
        {
            try
            {
                action?.Invoke(arg1, arg2, arg3);
            }
            catch (Exception e)
            {
                log.Exception(e);
            }
        }
    }
}