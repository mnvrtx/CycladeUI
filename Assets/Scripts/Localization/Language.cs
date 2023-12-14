using System.Collections.Generic;

//should be this namespace
namespace CycladeLocalization.Definition
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