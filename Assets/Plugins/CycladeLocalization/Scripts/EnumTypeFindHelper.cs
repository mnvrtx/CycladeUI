using System;
using System.Linq;
using Shared.Utils;

namespace CycladeLocalization
{
    public static class EnumTypeHelper
    {
        private static Cache<Type> TypeLang = new(() => CodeHelpers.FindTypesWith(q => q.FullName == "CycladeLocalization.Definition.Language" && q.IsEnum).Single());
        private static Cache<Type> TypeArea = new(() => CodeHelpers.FindTypesWith(q => q.FullName == "CycladeLocalization.Definition.Area" && q.IsEnum).Single());

        public static Type FindLang() => TypeLang.Value;

        public static Type FindArea() => TypeArea.Value;
    }
}