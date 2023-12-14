using System.Collections.Generic;

namespace CycladeLocalization.Definition //should be this namespace
{
    public enum Language
    {
        EN,
        RU,
    }

    public static class LanguageExt
    {   
        public static readonly Dictionary<Language, string> Names = new()
        {
            { Language.EN, "English" },
            { Language.RU, "Русский" },
        };
    }
}