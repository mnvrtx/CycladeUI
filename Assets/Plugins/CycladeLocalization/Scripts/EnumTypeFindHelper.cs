using System;
using System.Linq;
using CycladeBase.Utils;

namespace CycladeLocalization
{
    public static class EnumTypeHelper
    {
        public static Type FindLang() => CycladeHelpers.FindTypesWith(q => q.FullName == "CycladeLocalization.Definition.Language" && q.IsEnum).Single();
        public static Type FindArea() => CycladeHelpers.FindTypesWith(q => q.FullName == "CycladeLocalization.Definition.Area" && q.IsEnum).Single();
    }
}