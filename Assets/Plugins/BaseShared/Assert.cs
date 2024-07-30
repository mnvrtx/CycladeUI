using System;
using System.Runtime.CompilerServices;

namespace Shared.Utils
{
    public static class Assert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void True(bool b, string message)
        {
            if (!b)
                throw new Exception($"Should be true: {message}");
        }
    }
}