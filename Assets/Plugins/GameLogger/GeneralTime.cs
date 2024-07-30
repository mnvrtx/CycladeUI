using System;
using System.Runtime.CompilerServices;

namespace BaseShared
{
    public static class GeneralTime
    {
        public static long NowMs
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        }
        
        public static long NowTicks
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => DateTime.UtcNow.Ticks;
        }
    }
}