using System;

namespace CycladeBase.PerformanceAnalyzer.Trackers.Base
{
    public class ValSuffix
    {
        public readonly Func<float, string> ValSuffixFunc;
        public readonly Func<long, string> LongValSuffixFunc;
        public readonly bool IsLong;

        public ValSuffix(Func<float, string> valSuffixFunc)
        {
            ValSuffixFunc = valSuffixFunc;
            IsLong = false;
        }

        public ValSuffix(Func<long, string> longValSuffixFunc)
        {
            LongValSuffixFunc = longValSuffixFunc;
            IsLong = true;
        }
    }
}