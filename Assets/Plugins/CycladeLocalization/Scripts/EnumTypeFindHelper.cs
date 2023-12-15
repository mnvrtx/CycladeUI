using System;
using System.Linq;
using CycladeBase.Utils;

namespace CycladeLocalization
{
    public static class EnumTypeHelper
    {
        private static Cache<Type> TypeLang = new(() => CycladeHelpers.FindTypesWith(q => q.FullName == "CycladeLocalization.Definition.Language" && q.IsEnum).Single());
        private static Cache<Type> TypeArea = new(() => CycladeHelpers.FindTypesWith(q => q.FullName == "CycladeLocalization.Definition.Area" && q.IsEnum).Single());

        public static Type FindLang() => TypeLang.Value;

        public static Type FindArea() => TypeArea.Value;
    }
}